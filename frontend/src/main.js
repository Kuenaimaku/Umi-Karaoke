import { createApp } from 'vue'
import { 
	Quasar,
	Notify, 
	AddressbarColor, 
	LocalStorage,
  SessionStorage 
} from 'quasar'

// Import icon libraries
import '@quasar/extras/material-icons/material-icons.css'

// Import Quasar css
import 'quasar/src/css/index.sass'

import App from './App.vue';
import router from './router';

const app = createApp(App);
app.use(router);
app.use(Quasar, {
  plugins: {
		LocalStorage,
    SessionStorage,
    Notify,
		AddressbarColor
  },
  config: {
    notify: { }
  }
});
app.mount("#app");