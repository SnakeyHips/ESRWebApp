import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { AbsenceType } from '../../models/absencetype';

@Component
export default class EditAbsenceTypeComponent extends Vue {
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

	loading: boolean = false;
	failed: boolean = false;
	colours: string[] = ["Maroon", "Red", "Orange", "Yellow", "Olive", "Green", "Purple", "Fuchsia", "Lime",
		"Teal", "Aqua", "Blue", "Navy", "Gray", "Silver"];

	mounted() {
		this.loading = true;
		fetch('api/Admin/GetAbsenceTypeById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<AbsenceType>)
			.then(data => {
				this.absencetype = data;
				this.loading = false;
			});
	}

	editAbsenceType() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/UpdateAbsenceType', {
				method: 'PUT',
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

	cancel() {
		this.$router.push('/fetchadmin');
	}
}