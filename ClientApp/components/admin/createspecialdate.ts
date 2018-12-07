import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';

@Component
export default class CreateSpecialDateComponent extends Vue {	
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: value => !!value || 'Required',
		number: value => /[0-9]/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: value => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	specialdate: SpecialDate = {
		id: 0,
		name: "",
		date: ""
	}

	dateFormatted = "";
	failed: boolean = false;

	createSpecialDate() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/CreateSpecialDate', {
				method: 'POST',
				body: JSON.stringify(this.specialdate)
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

	formatDate() {
		this.dateFormatted = new Date(this.specialdate.date).toLocaleDateString();
	}

	clear() {
		this.$refs.form.reset();
	}

	cancel() {
		this.$router.push('/fetchadmin');
	}
}