import './css/site.css';
import 'bootstrap';
import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

const routes = [
    { path: '/', component: require('./components/home/home.vue.html').default },
		{ path: '/fetchemployee', component: require('./components/employee/fetchemployee.vue.html').default },
		{ path: '/createemployee', component: require('./components/employee/createemployee.vue.html').default },
		{ path: '/editemployee/:id', component: require('./components/employee/editemployee.vue.html').default },
		{ path: '/fetchsession', component: require('./components/session/fetchsession.vue.html').default },
		{ path: '/createsession', component: require('./components/session/createsession.vue.html').default },
		{ path: '/editsession/:id', component: require('./components/session/editsession.vue.html').default },
		{ path: '/rostersession/:id', component: require('./components/session/rostersession.vue.html').default },
		{ path: '/fetchabsence', component: require('./components/absence/fetchabsence.vue.html').default },
		{ path: '/createabsence', component: require('./components/absence/createabsence.vue.html').default },
		{ path: '/editabsence/:id', component: require('./components/absence/editabsence.vue.html').default },
];

new Vue({
    el: '#app-root',
    router: new VueRouter({ mode: 'history', routes: routes }),
    render: h => h(require('./components/app/app.vue.html').default)
});
