# GenChessImage
Console EXE to generate a chess board image based on FEN input.

Chess piece image sets from [http://ixian.com/chess/jin-piece-sets/](http://ixian.com/chess/jin-piece-sets/). 

**Dependencies**
.Net 4.5

**Config File**
Most settings in the config file. 

```html
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
  <appSettings>
    <!-- Name of the chess font image set used  -->
    <add key="ChessFont" value="alpha-02" />
    <!-- tile size. This sets the size of the chess font image set used 
    which in turn dictates the total size of the board. Board size =  tile size * 8  -->
    <add key="TileSize" value="40" />
    <!-- the directory to same images into  -->
    <add key="ImageDirectory" value="Images" />
    <!-- RGB color setting  -->
    <add key="BlackTileColor" value="209 , 139, 70" />
    <!-- RGB color setting  -->
    <add key="WhiteTileColor" value="255, 207, 161" />
    <!-- RGB color setting  -->
    <add key="BackGroundColor" value="252, 250, 227" />
    <!-- RGB color setting  -->
    <add key="TextColor" value="0, 0, 0" />
    <!-- RGB color setting  -->
    <add key="BorderColor" value="0, 0, 0" />
    <!-- RGB color setting  -->
    <add key="TileBorderColor" value="0, 0, 0" />
    <!-- Do you want a 1px border around each tile?
    <add key="DrawBorderTiles" value="false" />
  </appSettings>
</configuration>
```

**Example Usage**
Input requres  three arguments: fen, filename, and whether to add text border.

#### Sample Call
string[] args = {"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "chessboard", "true"};
GenChessImage.exe args

#### Sample Board
![Sample Board](https://pbs.twimg.com/media/ChdRP30WIAEbhAw.jpg "Sample Board")
