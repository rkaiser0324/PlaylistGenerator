# PlaylistGenerator

This project was spurred by me finding an ancient iPod in a drawer with some great playlists on it.  I had the music still, but it had been moved around and cleaned up significantly in the meantime - so it wasn't in my iTunes library anymore.  I wanted to recreate these playlists with my current iTunes so I could sync them to my devices.

## Building

Compiles with Visual Studio Community 2017 on Windows 10.

## Usage

1. Connect the iPod to your computer and launch iTunes
2. Select the playlist on the device, and go to File->Library->Export Playlist
3. Save the file as a Text file 
4. Run PlaylistGenerator with this syntax:
```dos
> PlaylistGenerator C:\path\to\oldplaylist.txt C:\path\to\Music
```
5. PlaylistGenerator will parse the playlist file and use the Windows Search API to attempt to find each corresponding MP3 file under the specified Music directory.  Since it searches by file attributed, not filenames, it worked pretty well with my music library (accuracy of > 90%).  It will then create an M3U file in the current directory with the files it found.
6. In iTunes, go to File->Library->Import Playlist and select the M3U file.

Enjoy!
