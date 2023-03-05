# Umi Karaoke

This repo contains a .NET 7 Web API backend and a Vue 3 frontend app meant to serve as a playlist manager for Karaoke songs.

## Backend

Utilizes EntityFramework, AspNET, and SignalR to provide a WebAPI for listing songs, and adding/removing them from a Queue. Swagger documentation is provided.
The songs are expected to have CDG and MP3 files named the same respectively, and are expected to all live within a single directory.

## Frontend

Vue 3 Frontend built with vite, quasar, cdgraphics, and gsap. 
- The home route is meant to be viewed on a phone, and handles listing and filtering songs, and adding them to a queue. 
  - Users can also remove songs from the queue if they haven't been played yet. 
- The `/video` route is meant to be viewed on a larger format, and uses the cdgraphics library to play cdg graphics sync'd with an accompanying MP3 file.

both routes use SignalR for messaging about the queue, and any errors from the backend.