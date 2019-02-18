import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Role } from '../../models/role';

@Component
export default class CreateRoleComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	role: Role = {
		id: 0,
		name: "",
		count: true,
	}

	failed: boolean = false;
	choices: boolean[] = [true, false];

	createRole() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/CreateRole', {
				method: 'POST',
				body: JSON.stringify(this.role)
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