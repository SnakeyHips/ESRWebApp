import './css/site.css';
import 'vuetify/dist/vuetify.min.css';
import 'material-design-icons-iconfont/dist/material-design-icons.css';
import 'typeface-roboto/index.css';
import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuetify from 'vuetify';

Vue.use(VueRouter);
Vue.use(Vuetify);

const routes = [
	{ path: '/', component: require('./components/home/home.vue.html').default },
	{ path: '/fetchsession', component: require('./components/session/fetchsession.vue.html').default },
	{ path: '/createsession', component: require('./components/session/createsession.vue.html').default },
	{ path: '/editsession/:id', component: require('./components/session/editsession.vue.html').default },
	{ path: '/rostersession/:id', component: require('./components/roster/rostersession.vue.html').default },
	{ path: '/fetchemployee', component: require('./components/employee/fetchemployee.vue.html').default },
	{ path: '/createemployee', component: require('./components/employee/createemployee.vue.html').default },
	{ path: '/editemployee/:id', component: require('./components/employee/editemployee.vue.html').default },
	{ path: '/viewemployee/:id', component: require('./components/employee/viewemployee.vue.html').default },
	{ path: '/fetchteam', component: require('./components/team/fetchteam.vue.html').default },
	{ path: '/createteam', component: require('./components/team/createteam.vue.html').default },
	{ path: '/editteam/:id', component: require('./components/team/editteam.vue.html').default },
	{ path: '/viewteam/:id', component: require('./components/team/viewteam.vue.html').default },
	{ path: '/fetchroster', component: require('./components/roster/fetchroster.vue.html').default },
	{ path: '/fetchabsence', component: require('./components/absence/fetchabsence.vue.html').default },
	{ path: '/createabsence', component: require('./components/absence/createabsence.vue.html').default },
	{ path: '/editabsence/:id', component: require('./components/absence/editabsence.vue.html').default },
	{ path: '/fetchadmin', component: require('./components/admin/fetchadmin.vue.html').default },
	{ path: '/createspecialdate', component: require('./components/admin/createspecialdate.vue.html').default },
	{ path: '/editspecialdate/:id', component: require('./components/admin/editspecialdate.vue.html').default },
	{ path: '/createrole', component: require('./components/admin/createrole.vue.html').default },
	{ path: '/editrole/:id', component: require('./components/admin/editrole.vue.html').default },
	{ path: '/createtemplate', component: require('./components/admin/createtemplate.vue.html').default },
	{ path: '/edittemplate/:id', component: require('./components/admin/edittemplate.vue.html').default },
	{ path: '/createskill', component: require('./components/admin/createskill.vue.html').default },
	{ path: '/editskill/:id', component: require('./components/admin/editskill.vue.html').default },
	{ path: '/createsite', component: require('./components/admin/createsite.vue.html').default },
	{ path: '/editsite/:id', component: require('./components/admin/editsite.vue.html').default },
	{ path: '/createabsencetype', component: require('./components/admin/createabsencetype.vue.html').default },
	{ path: '/editabsencetype/:id', component: require('./components/admin/editabsencetype.vue.html').default },
];

new Vue({
	el: '#app-root',
	router: new VueRouter({ mode: 'history', routes: routes }),
	render: h => h(
		require('./components/app/app.vue.html').default
	),
});
