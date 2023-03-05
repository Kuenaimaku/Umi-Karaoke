<script setup>
import {ref, reactive, onMounted, computed} from "vue";
import { useQuasar } from 'quasar'
import queueHub from "../hub/QueueHub"

import gsap from 'gsap'

import SongListItem from '../components/SongListItem.vue'
import QueueListItem from '../components/QueueListItem.vue'

const $q = useQuasar();

onMounted(async () => {
  queueHub.client.on("SongAddedToQueue", function(queueDTO) {
    state.queue.push(queueDTO);
    $q.notify({
      message: `${queueDTO.user} added ${queueDTO.song.title} - ${queueDTO.song.artist} to the queue.`,
      color: 'accent',
      textColor: 'white',
      icon: 'playlist_add',
      position: 'top',
      progress: true,
      timeout: 2000,
      group: false
    })
  })

  queueHub.client.on("FailedToAddSong", function(message) {
    $q.notify({
      message: `Failed to add song! ${message}`,
      type: 'negative',
      position: 'top',
      progress: true,
      timeout: 5000,
      group: false
    })
  })

  queueHub.client.on("SongRemovedFromQueue", function(queueDTO) {
    const x = state.queue.find(q => q.id === queueDTO.id);
    const index = state.queue.indexOf(x);
    state.queue.splice(index, 1);

    $q.notify({
      message: `${queueDTO.user} removed ${queueDTO.song.title} - ${queueDTO.song.artist} from the queue.`,
      color: 'accent',
      textColor: 'white',
      icon: 'playlist_remove',
      position: 'top',
      progress: true,
      timeout: 2000,
      group: false
    })
  })

  queueHub.client.on("FailedToRemoveSong", function(message) {
    $q.notify({
      message: `Could not remove from the queue! ${message}`,
      type: 'negative',
      position: 'top',
      progress: true,
      timeout: 5000,
      group: false
    })
  })

  queueHub.client.on("SongSetToNowPlaying", function(queueDTO) {
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
      color: 'accent',
      textColor: 'white',
      icon: 'music_note',
      position: 'top',
      progress: true,
      timeout: 2000,
      group: false,
      multiLine: true,
      html: true
    })
  })

  queueHub.start();

  let response = await fetch(`${import.meta.env.VITE_API_URL}/api/Songs/`)
  let data = await response.json();
  state.songs = data;

  response = await fetch(`${import.meta.env.VITE_API_URL}/api/Queue/`)
  data = await response.json();
  state.queue = data;
  state.loading = false;
})

const state = reactive({
  loading: true,
  songs: [],
  queue: []
});

const rightDrawerOpen = ref(false);
const name = ref("");
const filter = ref("");

const filterSongs = computed(() => {
  if(filter.value === "") {return state.songs}
  return state.songs.filter(function (song) { return song.title.toLowerCase().includes(filter.value.toLowerCase())})
})

function onBeforeEnter(el) {
  el.style.opacity = 0
  el.style.height = 0
}

function onEnter(el, done) {
  gsap.to(el, {
    opacity: 1,
    height: '60px',
    delay: el.dataset.index * 0.15,
    onComplete: done
  })
}

function onEnterQueue(el, done) {
  gsap.to(el, {
    opacity: 1,
    height: '71px',
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

function addSong(song) {
  queueHub.client.invoke('AddSong', name.value, song.id).catch(function (error) {
    return console.error(error.toString());
  })
}

function removeSong(id) {
  queueHub.client.invoke('RemoveSong', id).catch(function (error) {
    return console.error(error.toString());
  })
}

function toggleRightDrawer () {
  rightDrawerOpen.value = !rightDrawerOpen.value
}
</script>

<template>
  <q-layout view="hHh lpR fFf">

    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-toolbar-title>
          <q-avatar>
            <img src="/logo.svg" >
          </q-avatar>
          Umi Karaoke
        </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer
        v-model="rightDrawerOpen"
        side="right" 
        overlay 
        behavior="mobile" 
        elevated>
        <q-scroll-area style="height: calc(100% - 150px); margin-top:150px; border-right: 1px solid #ddd">
          <transition-group 
          tag="div"
          :css="false"
          @before-enter="onBeforeEnter"
          @enter="onEnterQueue"
          @leave="onLeave">
            <aside v-for="item in state.queue" :key="item.id">
                <queue-list-item 
                  :item="item" 
                  :canRemove="item.user === name && item.status != 1"
                  @remove-song=" removeSong(item.id)"
                />
            </aside>
          </transition-group>
        </q-scroll-area>

        <q-img class="absolute-top" src="/drawer-header.png" style="height: 150px">
          <div class="absolute-top bg-transparent">
            <q-input filled v-model="name" label="Name" bg-color="white" stack-label>
              <template v-slot:prepend>
                <q-icon name="person" />
              </template>
            </q-input>
          </div>
          <div class="absolute-bottom bg-transparent">
            <div class="text-h6">Current Queue</div>
          </div>
        </q-img>
      </q-drawer>

    <q-page-container>
      <q-page>
        <transition-group
          tag="div"
          :css="false"
          @before-enter="onBeforeEnter"
          @enter="onEnter"
          @leave="onLeave">
          <aside v-for="song in filterSongs" :key="song.id">
            <song-list-item 
              :song="song" 
              :canAdd="name != ''" 
              @add-song="addSong(song)"
            />
          </aside>
        </transition-group>
      </q-page>
      <q-inner-loading :showing="state.loading">
          <q-spinner-gears size="10%" color="primary" />
        </q-inner-loading>
    </q-page-container>

    <q-footer elevated>
        <q-toolbar>
          <q-input dark dense standout v-model="filter" class="full-width" placeholder="Filter by Song Title...">
          <template v-slot:append>
            <q-icon v-if="filter === ''" name="search" />
            <q-icon v-else name="clear" class="cursor-pointer" @click="filter = ''" />
          </template>
        </q-input>
        <q-btn dense flat round icon="menu" style="margin-left:12px;" @click="toggleRightDrawer" />
        </q-toolbar>
      </q-footer>
  </q-layout>
</template>

<style scoped>

.q-page-container {
  background:#2E303D !important;
}
</style>
