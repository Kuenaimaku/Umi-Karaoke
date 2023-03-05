import { createWebHistory, createRouter } from "vue-router";
import User from "../views/User.vue";
import Video from "../views/Video.vue";

const routes = [
  {
    path: "/",
    name: "User",
    component: User,
  },
  {
    path: "/video",
    name: "Video",
    component: Video,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;