
import landingComponent from './components/landing-component.vue';

var appMain = new Vue({
    el: '#app',
    components: {
        "app-view": landingComponent
    }
});

export { appMain as default }