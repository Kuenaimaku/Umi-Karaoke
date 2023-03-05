<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  song: Object,
  canAdd: Boolean
})

const emit = defineEmits(['addSong'])

function addSong(){
  emit('addSong', props.song)
}

const classes = computed(() => {
  if (!props.canAdd) {return "invisible disabled"}
  return ""
} )

</script>

<template>
  <q-card flat square bordered>
    <q-item>
      <q-item-section>
        <q-item-label>{{ song.title }}</q-item-label>
        <q-item-label caption>
          {{ song.artist }}
        </q-item-label>
      </q-item-section>
      <q-item-section avatar>
        <q-btn round color="primary" text-color="white" icon="add" :class="classes" @click="addSong"/>
      </q-item-section>
    </q-item>
  </q-card>
</template>

<style scoped>

</style>
