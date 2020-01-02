# <img src="https://i.imgur.com/4XAjsxJ.png" style="max-width:120px"></img> MinoriTool

Simple tool to batch convert compressed or uncompressed .gim files in Toradora! Portable (and probably other games) to PNG using Sony's official GimConv tool and gunzip.

Originally created for and only tested with Toradora! Portable. Other .gim files probably work too.

## Usage

1. Create an `input` folder and an `gimconv` in the same directory as `MinoriTool.exe`.
2. Copy your .gim files into the newly created `input` folder. (You can dump these from Toradora! Portable using [TigerXDragon](https://github.com/SH4FS0c13ty/TigerXDragon))
3. Copy GimConv.exe and it's required files into the newly created `gimconv` folder. (GimConv was previously officially distributed with the PS3 Custom Theme Toolbox and the PSP Custom Theme Toolbox, but official downloads no longer exist so you'll need to find GimConv yourself)
4. Ensure you have a WSL instance (or some other alternative) accessible by `bash` from the Windows command prompt and that running `bash -c "which gunzip"` from the Windows command prompt successfully locates gunzip.
5. Run `MinoriTool.exe`.