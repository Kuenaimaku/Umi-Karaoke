<script setup>
import {ref, reactive, onMounted, computed} from "vue";
import { useQuasar } from 'quasar'
import gsap from 'gsap'

const $q = useQuasar();

onMounted(async () => {
  let lockResolver;
  if (navigator && navigator.locks && navigator.locks.request) {
      const promise = new Promise((res) => {
          lockResolver = res;
      });

      navigator.locks.request('unique_lock_name', { mode: "shared" }, () => {
          return promise;
      });
  }
})

</script>


<template>
  <div id="app">
    <router-view></router-view>
  </div>
</template>

<style scoped>

</style>
