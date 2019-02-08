import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Skill } from '../../models/skill';

@Component
export default class CreateSkillComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	skill: Skill = {
		id: 0,
		role: "",
		name: ""
	}

	loading: boolean = false;
	failed: boolean = false;
	roles: string[] = [];

	mounted() {
		this.loading = true;
		fetch('api/Admin/GetRoleNames')
			.then(response => response.json() as Promise<string[]>)
			.then(data => {
				this.roles = data;
				this.loading = false;
			});
	}

	createSkill() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/CreateSkill', {
				method: 'POST',
				body: JSON.stringify(this.skill)
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