import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { Site } from '../../models/site';
import { SelectedDate } from '../../models/selecteddate';

@Component
export default class CreateSessionComponent extends Vue {
	@Prop(SelectedDate) selecteddate!: SelectedDate;
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	session: Session = {
		id: 0,
		date: "",
		day: "",
		type: "",
		site: "",
		time: "",
		lod: 0,
		chairs: 0,
		occ: 0,
		estimate: 0,
		holiday: 0,
		note: "",
		template: "",
		staffCount: 0,
		state: 0,
		employees: []
	}

	dateFormatted = "";
	types: string[] = ["Community", "MDC"];
	sites: Site[] = [];
	times: string[] = [];
	templates: string[] = [];
	failed: boolean = false;

	mounted() {
		this.loadTemplates();
	}

	createSession() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Session/Create', {
				method: 'POST',
				body: JSON.stringify(this.session)
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						this.failed = true;
					} else {
						this.selecteddate.date = this.session.date;
						this.$router.push('/fetchsession');
					}
				})
		}
	}

	loadSites(type: string) {
		fetch('api/Session/GetSites?type=' + type)
			.then(response => response.json() as Promise<Site[]>)
			.then(data => {
				this.sites = data;
			});
	}

	loadTimes(site: string) {
		for (var i = 0; i < this.sites.length; i++) {
			if (this.sites[i].name === site) {
				this.times = this.sites[i].times.split('/');
			}
		}
	}

	loadTemplates() {
		fetch('api/Admin/GetTemplateNames')
			.then(response => response.json() as Promise<string[]>)
			.then(data => {
				this.templates = data;
			})
	}

	formatDate() {
		this.dateFormatted = new Date(this.session.date).toLocaleDateString();
	}

	clear() {
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchsession');
	}
}