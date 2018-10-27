PlaylistGenerator
=================

Given a text-format playlist export from iTunes, parse it and use the Windows Search API to attempt to find each corresponding MP3 files in the current Music directory.  

Then recreate the playlist, to the extent possible, as an M3U file in the current directory which can then be imported back into iTunes.

```
> PlaylistGenerator C:\path\to\oldplaylist.txt C:\path\to\Music
```
