<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  item: Object,
  canRemove: Boolean
})

const emit = defineEmits(['removeSong'])

function removeSong(){
  emit('removeSong', props.item.song)
}

const classes = computed(() => {
  if (props.item.status == 2 || props.item.status == 3) {return "hidden"}
  if (props.item.status == 1) {return "bg-amber-3"}
  return ""
} )
</script>

<template>
  <q-card square flat bordered :class="classes">
    <q-item>
      <q-item-section>
        <q-item-label> {{ item.song.title }} </q-item-label>
        <q-item-label caption>
          {{ item.song.artist }}
        </q-item-label>
        <q-item-label caption>
          {{ item.user }}
        </q-item-label>
      </q-item-section>
      <q-item-section avatar>
        <q-btn color="negative" round text-color="white" icon="delete" v-if="canRemove" @click="removeSong"/>
      </q-item-section>
    </q-item>
  </q-card>
</template>

<style scoped>

</style>
