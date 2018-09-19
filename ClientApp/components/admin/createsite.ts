import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Site } from '../../models/site';

@Component
export default class CreateSiteComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: value => !!value || 'Required',
		number: value => /[0-9]/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: value => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	site: Site = {
		id: 0,
		name: "",
		type: "",
		times: ""
	}

	failed: boolean = false;
	types: string[] = ["Community", "MDC"];

	createSite() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/CreateSite', {
				method: 'POST',
				body: JSON.stringify(this.site)
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						this.failed = true;
					} else {
						this.$router.push('/fetchadmin');
					}
				})
		}
	}

	clear() {
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchadmin');
	}
}
