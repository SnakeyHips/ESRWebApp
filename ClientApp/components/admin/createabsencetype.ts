import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { AbsenceType } from '../../models/absencetype';

@Component
export default class CreateAbsenceTypeComponent extends Vue {
	$refs!: {
		form: HTMLFormElement
	}

	rules: object = {
		required: (value: string) => !!value || 'Required',
		number: (value: string) => /^\d+(\d{1,2})?$/.test(value) || 'Value must be number e.g. "8" or "10"',
		decimal: (value: string) => /^\d+(\.\d{1,2})?$/.test(value) || 'Value must be decimal e.g. "8.0" or "7.5"'
	}

	absencetype: AbsenceType = {
		id: 0,
		name: "",
		colour: "",
	}

	failed: boolean = false;
	colours: string[] = ["Red", "Pink", "Purple", "Indigo", "Blue", "Teal", "Green", "Orange"];

	createAbsenceType() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/CreateAbsenceType', {
				method: 'POST',
				body: JSON.stringify(this.absencetype)
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