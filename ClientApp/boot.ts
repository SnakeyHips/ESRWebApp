
import './css/site.css';
import 'vuetify/dist/vuetify.min.css';
import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuetify from 'vuetify';

Vue.use(VueRouter);
Vue.use(Vuetify, {
	theme: {
		primary: '#ed1c24',
		secondary: '#f36368',
		accent: '#af0e14', 
		error: '#af0e14'
	}
});

const routes = [
	{ path: '/', component: require('./components/home/home.vue.html').default },
	{ path: '/fetchsession', component: require('./components/session/fetchsession.vue.html').default },
	{ path: '/createsession', component: require('./components/session/createsession.vue.html').default },
	{ path: '/editsession/:id', component: require('./components/session/editsession.vue.html').default },
	{ path: '/rostersession/:id', component: require('./components/session/rostersession.vue.html').default },
	{ path: '/fetchemployee', component: require('./components/employee/fetchemployee.vue.html').default },
	{ path: '/createemployee', component: require('./components/employee/createemployee.vue.html').default },
	{ path: '/editemployee/:id', component: require('./components/employee/editemployee.vue.html').default },
	{ path: '/viewemployee/:id', component: require('./components/employee/viewemployee.vue.html').default },
	{ path: '/fetchroster', component: require('./components/roster/fetchroster.vue.html').default },
	{ path: '/fetchabsence', component: require('./components/absence/fetchabsence.vue.html').default },
	{ path: '/createabsence', component: require('./components/absence/createabsence.vue.html').default },
	{ path: '/editabsence/:id', component: require('./components/absence/editabsence.vue.html').default },
	{ path: '/fetchadmin', component: require('./components/admin/fetchadmin.vue.html').default },
	{ path: '/createspecialdate', component: require('./components/admin/createspecialdate.vue.html').default },
	{ path: '/editspecialdate/:id', component: require('./components/admin/editspecialdate.vue.html').default },
	{ path: '/createsite', component: require('./components/admin/createsite.vue.html').default },
	{ path: '/editsite/:id', component: require('./components/admin/editsite.vue.html').default },
];

new Vue({
    el: '#app-root',
    router: new VueRouter({ mode: 'history', routes: routes }),
    render: h => h(require('./components/app/app.vue.html').default)
});
