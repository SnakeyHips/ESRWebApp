import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Template } from '../../models/template';

@Component
export default class EditTemplateComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	template: Template = {
		id: 0,
		name: "",
		roles: "",
	}

	failed: boolean = false;
	loading: boolean = false;
	roles: string[] = [];
	templateRoles: string[] = [];

	mounted() {
		this.loading = true;
		fetch('api/Admin/GetTemplateById?id=' + this.$route.params.id)
			.then(response => response.json() as Promise<Template>)
			.then(data => {
				this.template = data;
				this.templateRoles = this.template.roles.split(',');
			});
		this.loadRoles();
		this.loading = false;
	}

	editTemplate() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			this.template.roles = this.templateRoles.join(',');
			fetch('api/Admin/UpdateTemplate', {
				method: 'PUT',
				body: JSON.stringify(this.template)
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

	loadRoles() {
		this.loading = true;
		fetch('api/Admin/GetRoleNames')
			.then(response => response.json() as Promise<string[]>)
			.then(data => {
				this.roles = data;
				this.loading = false;
			})
	}

	addRole() {
		if (this.templateRoles.length < 31) {
			this.templateRoles.push('');
		}
	}

	removeRole() {
		if (this.templateRoles.length > 1) {
			this.templateRoles.pop();
		}
	}

	clear() {
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchadmin');
	}
}