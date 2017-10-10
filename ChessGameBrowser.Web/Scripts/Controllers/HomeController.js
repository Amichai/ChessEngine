var app = angular.module('chessApp', []);

app.controller('homeCtrl', ['$scope', '$http',
    function($scope, $http) {

        $scope.test = 0;

        function isDark(x, y) {
            var a = x % 2 == 0;
            var b = y % 2 == 0;
            return a != b;
        }

        function getXY(cell) {
            var v = cell.classList[1];
            return [v[1], v[3]];
        }

        $scope.getClass = function (x, y) {
            if (isDark(x, y)) {
                return "dark-cell";
            }
            return "light-cell";
        }

        $scope.getTransform = function (x, y) {
            return "translate(" + x * 64 + "px, " + y * 64 + "px)";
        }


        $scope.boardState = [
            -4, -2, -3, -5, -6, -3, -2, -4,
            -1, -1, -1, -1, -1, -1, -1, -1,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1,
           4, 2, 3, 5, 6, 3, 2, 4
        ];

        var selectedCell;

        function deselect(cell) {
            var xy = getXY(cell);

            if (!isDark(xy[0], xy[1])) {
                cell.classList.add('light-cell');
                cell.classList.remove('selected-light');
            } else {
                cell.classList.add('dark-cell');
                cell.classList.remove('selected-dark');
            }
        }

        function hasPiece(cell) {
            if (cell.classList.length < 4) {
                return false;
            }

            var pieceClass = cell.classList[3];
            if (pieceClass.includes("white") || pieceClass.includes("black")) {
                return true;
            }

            pieceClass = cell.classList[2];

            if (pieceClass.includes("white") || pieceClass.includes("black")) {
                return true;
            }

            return false;
        }

        var postMoveDataStack = new Array();

        $scope.repeatPosition = function () {
            postMove(postMoveDataStack.pop());
        }

        $scope.navigateBackward = function () {
            postMoveDataStack.pop();
            postMove(postMoveDataStack.pop());
        }

        function updateBoard(result) {
            fenState = result;
            var newBoardState = [];
            for (var i = 0; i < result.length; i++) {
                var val = result[i];

                if (val === '/') {
                    continue;
                }

                if (val == ' ') {
                    break;
                }

                if (!isNaN(parseInt(val))) {
                    for (var j = 0; j < parseInt(val) ; j++) {
                        newBoardState.push(0);
                    }

                    continue;
                }

                switch (val) {
                    case "r":
                        newBoardState.push(-4);
                        break;
                    case "R":
                        newBoardState.push(4);
                        break;

                    case "n":
                        newBoardState.push(-2);
                        break;
                    case "N":
                        newBoardState.push(2);
                        break;

                    case "b":
                        newBoardState.push(-3);
                        break;
                    case "B":
                        newBoardState.push(3);
                        break;

                    case "q":
                        newBoardState.push(-5);
                        break;
                    case "Q":
                        newBoardState.push(5);
                        break;

                    case "k":
                        newBoardState.push(-6);
                        break;
                    case "K":
                        newBoardState.push(6);
                        break;

                    case "p":
                        newBoardState.push(-1);
                        break;
                    case "P":
                        newBoardState.push(1);
                        break;
                    default:
                }
            }

            $scope.boardState = newBoardState;
            init();
        };

        var fenState = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        var cols = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

        function col(x) {
            return cols[x];
        }

        function row(x) {
            return (8 - x).toString()[0];
        }

        $scope.moveList = [];

        $scope.evaluation = "";
        $scope.bestLine = "";
        $scope.autoMove = false;

        function postMove(data) {
            data.autoMove = $scope.autoMove;
            $http.post(baseUrl + 'api/home/postMove', data).then(function (result) {
                if (result != undefined && result.data.IsValid == true) {
                    $scope.moveList.push(data.move);
                    if (result.data.ComputerMove != "" && result.data.ComputerMove != undefined) {
                        $scope.moveList.push(result.data.ComputerMove);
                    }

                    updateBoard(result.data.FEN);

                    postMoveDataStack.push(data);

                    $scope.availableMoves = result.data.Moves.Moves;

                    selectedCell = undefined;

                    $scope.evaluation = "...";
                    $scope.bestLine = "";
                    $http.get(baseUrl + 'api/home/evaluation?boardState=' + result.data.FEN).then(function(result) {
                        $scope.evaluation = result.data.Eval;
                        $scope.bestLine = result.data.BestLine;
                    });
                }
            });
        }

        function makeMove(cell1, cell2) {
            var c1 = getXY(cell1);
            var c2 = getXY(cell2);

            var obj = new Object();
            obj.fen = fenState;
            obj.move = col(c1[0]) + row(c1[1]) + "-" + col(c2[0]) + row(c2[1]);

            postMove(obj);
        }

        $scope.selectMove = function(move) {
            var obj = new Object();
            obj.fen = fenState;
            obj.move = move.Move;

            postMove(obj);
        }

        function validateMove(cell1, cell2) {
            if ($scope.availableMoves == undefined || $scope.availableMoves.length == 0) {
                return '';
            }

            var c1 = getXY(cell1);
            var c2 = getXY(cell2);

            var move = col(c1[0]) + row(c1[1]) + "-" + col(c2[0]) + row(c2[1]);

            var match = findMatch($scope.availableMoves, function(element) { return element.Move == move; });

            if (move == $scope.availableMoves[0].Move) {
                return 'correct';
            } else if (match != undefined) {
                return move + ' - close ' + (match.White + match.Draw + match.Black);
            }
            return 'wrong';
        }

        function findMatch(array, predicate) {
            for (var i = 0; i < array.length; i++) {
                if (predicate(array[i])) {
                    return array[i];
                }
            }

            return undefined;
        }

        $scope.moveResult = "";

        $scope.selectCell = function ($event) {
            var target = $event.currentTarget;

            if (selectedCell != undefined) {
                deselect(selectedCell);

                if ($scope.restrictedPlay) {
                    $scope.moveResult = validateMove(selectedCell, target);

                    if ($scope.moveResult != 'correct' && $scope.moveResult != '') {

                        selectedCell = undefined;

                        return;
                    }
                }

                makeMove(selectedCell, target);

                selectedCell = undefined;
            }

            if (!hasPiece(target)) {
                return;
            }

            var xy = getXY(target);

            if (!isDark(xy[0], xy[1])) {
                target.classList.remove('light-cell');
                target.classList.add('selected-light');
            } else {
                target.classList.remove('dark-cell');
                target.classList.add('selected-dark');
            }

            selectedCell = target;
        }

        angular.element(document).ready(function () {
            init();
        });

        // at the bottom of your controller
        var init = function () {

            $('.cell').removeClass('white-pawn').removeClass('white-knight').removeClass('white-bishop')
                .removeClass('white-rook').removeClass('white-queen').removeClass('white-king');

            $('.cell').removeClass('black-pawn').removeClass('black-knight').removeClass('black-bishop')
                .removeClass('black-rook').removeClass('black-queen').removeClass('black-king');

            for (var i = 0; i < $scope.boardState.length; i++) {
                var x = i % 8;
                var y = Math.floor(i / 8);

                var v = $scope.boardState[i];

                var colorPrefix = v < 0 ? 'black' : 'white';
                var target = $('.X' + x + 'Y' + y);
                switch (Math.abs(v)) {
                    case 1:
                        target.addClass(colorPrefix + '-pawn');
                        break;
                    case 2:
                        target.addClass(colorPrefix + '-knight');
                        break;
                    case 3:
                        target.addClass(colorPrefix + '-bishop');
                        break;
                    case 4:
                        target.addClass(colorPrefix + '-rook');
                        break;
                    case 5:
                        target.addClass(colorPrefix + '-queen');
                        break;
                    case 6:
                        target.addClass(colorPrefix + '-king');
                        break;

                default:
                }
            }
        };
    }]);
