
import defaultExport from './components/dashboard-component.vue';

var appMain = new Vue({
    el: '#app',
    components: {
        "app-view": defaultExport
    }
});

export { appMain as default }