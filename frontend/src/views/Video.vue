<script setup>
import {ref, onMounted, reactive, toRaw} from "vue";
import { useQuasar } from 'quasar'
import gsap from 'gsap'
import queueHub from "../hub/QueueHub"
import CDGraphics from 'cdgraphics';

import QueueListItem from '../components/QueueListItem.vue'

const $q = useQuasar();

const state = reactive({
  loading: true,
  songs: [],
  queue: []
});

const cdg = new CDGraphics();

function onBeforeEnter(el) {
  el.style.opacity = 0
  el.style.height = 0
}

function onEnter(el, done) {
  gsap.to(el, {
    opacity: 1,
    height: '70px',
    delay: el.dataset.index * 0.15,
    onComplete: done
  })
}

function onLeave(el, done) {
  gsap.to(el, {
    opacity: 0,
    height: 0,
    delay: el.dataset.index * 0.15,
    onComplete: done
  })
}

async function nextSong() {
  const queueDTOProxy = state.queue.find(q => q.status === 0);
  if (queueDTOProxy){
    const queueDTO = toRaw(queueDTOProxy);
    queueHub.client.invoke('SetNowPlaying', queueDTO).catch(function (error) {
    return console.error(error.toString());
    })
  } else {
    $q.notify({
      message: `There are no queued songs. Add more!`,
      type: 'negative',
      position: 'bottom',
      progress: true,
      timeout: 5000,
      group: false
    })
  }

}

async function changeCDG(songId){
  const audio = document.getElementById('audio')
  const canvas = document.getElementById('canvas')
  const ctx = canvas.getContext('2d')


  let frameId;

  const doRender = time => {
    const frame = cdg.render(time, { forceKey: false })
    if (!frame.isChanged) return

    createImageBitmap(frame.imageData)
      .then(bitmap => {
        ctx.clearRect(0, 0, canvas.clientWidth, canvas.clientHeight)
        ctx.imageSmoothingEnabled = false
        ctx.drawImage(bitmap, 0, 0, canvas.clientWidth, canvas.clientHeight)
      })
  }

    // render loop
  const pause = () => cancelAnimationFrame(frameId)
  const play = () => {
    frameId = requestAnimationFrame(play)
    doRender(audio.currentTime)
  }

  // follow audio events (depending on your app, not all are strictly necessary)
  audio.addEventListener('play', play)
  audio.addEventListener('pause', pause)
  audio.addEventListener('ended', pause)
  audio.addEventListener('seeked', () => doRender(audio.currentTime))

  //download mp3 and create new blob
  const mp3response = await fetch(`${import.meta.env.VITE_API_URL}/api/Songs/${songId}/mp3`);
  const mp3buffer = await mp3response.arrayBuffer();
  const blob = new Blob([mp3buffer], { type: "audio/mp3" });
  const url = window.URL.createObjectURL(blob);

  // download and load cdg file
  fetch(`${import.meta.env.VITE_API_URL}/api/Songs/${songId}/cdg`)
    .then(response => response.arrayBuffer())
    .then(buffer => {
      cdg.load(buffer);
      audio.src = url;
      cctx.clearRect(0, 0, canvas.clientWidth, canvas.clientHeight);
  })
}

onMounted(async () => {
  //signalR
  queueHub.client.on("SongAddedToQueue", function(queueDTO) {
    state.queue.push(queueDTO);
    $q.notify({
      message: `${queueDTO.user} added ${queueDTO.song.title} - ${queueDTO.song.artist} to the queue.`,
      color: 'primary',
      textColor: 'white',
      icon: 'playlist_add',
      position: 'bottom',
      progress: true,
      timeout: 2000,
      group: false
    })
  })

  queueHub.client.on("SongRemovedFromQueue", function(queueDTO) {
    const x = state.queue.find(q => q.id === queueDTO.id);
    const index = state.queue.indexOf(x);
    state.queue.splice(index, 1);

    $q.notify({
      message: `${queueDTO.user} removed ${queueDTO.song.title} - ${queueDTO.song.artist} from the queue.`,
      color: 'primary',
      textColor: 'white',
      icon: 'playlist_remove',
      position: 'bottom',
      progress: true,
      timeout: 2000,
      group: false
    })
  })

  queueHub.client.on("SongSetToNowPlaying", async function(queueDTO) {
    const previous = state.queue.find(q => q.status === 1);
    const prevIndex = state.queue.indexOf(previous);
    previous.status = 2;

    const nowPlaying = state.queue.find(q => q.id === queueDTO.id);
    const nowPlayingIndex = state.queue.indexOf(nowPlaying);
    nowPlaying.status = 1;
    
    state.queue[prevIndex] = previous;
    state.queue[nowPlayingIndex] = nowPlaying

    $q.notify({
      message: `Now Playing: ${queueDTO.song.title} - ${queueDTO.song.artist}<br/><span class="text-caption">Requested By ${queueDTO.user}</span>`,
      color: 'primary',
      textColor: 'white',
      icon: 'music_note',
      position: 'bottom',
      progress: true,
      timeout: 2000,
      group: false,
      multiLine: true,
      html: true
    })

    await changeCDG(queueDTO.song.id);
  })

  queueHub.client.on("FailedToSetNowPlaying", function(message) {
    $q.notify({
      message: `Failed to set song to Now Playing! ${message}`,
      type: 'negative',
      position: 'top',
      progress: true,
      timeout: 5000,
      group: false
    })
  })

  queueHub.start();

  //Data
  let response = await fetch(`${import.meta.env.VITE_API_URL}/api/Queue`)
  let data = await response.json();
  state.queue = data;
  state.loading = false;
  let nowPlaying = data.find(q => q.status === 1);
  if(nowPlaying){
    await changeCDG(nowPlaying.songId);
  }

})

</script>


<template>
  <q-layout view="hHh lpR fFf">
    
    <q-drawer show-if-above side="right" behavior="desktop" :width=300>
      <q-scroll-area style="height: calc(100% - 161px); margin-top:65px; border-right: 1px solid #ddd">
        <transition-group 
          tag="div"
          :css="false"
          @before-enter="onBeforeEnter"
          @enter="onEnter"
          @leave="onLeave">
          <aside v-for="item in state.queue" :key="item.id">
            <queue-list-item :item="item" :canRemove="false"/>
          </aside>
        </transition-group> 
      </q-scroll-area>
      <audio id="audio" controls></audio>
      <q-btn class="full-width" square color="accent" @click="nextSong()">Next Song</q-btn>

      <q-img class="absolute-top" src="/drawer-header.png" style="height: 65px">
        <div class="absolute-top bg-transparent">
        </div>
        <div class="absolute-bottom bg-transparent">
          <div class="text-h6">Current Queue</div>
        </div>
      </q-img>
    </q-drawer>

    <q-page-container>
      <q-page>
        <canvas id="canvas" width="1620" height="1080" class="absolute-center"/>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<style>

html, #canvas {
  background:#000;
}
</style>