import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Skill } from '../../models/skill';

@Component
export default class EditSkillComponent extends Vue {
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
	roles: string[] = ["SV", "DRI", "RN", "CCA"];

	mounted() {
		this.loading = true;
		fetch('api/Admin/GetSkillById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Skill>)
			.then(data => {
				this.skill = data;
				this.loading = false;
			});
	}

	editSkill() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/UpdateSkill', {
				method: 'PUT',
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

	cancel() {
		this.$router.push('/fetchadmin');
	}
}