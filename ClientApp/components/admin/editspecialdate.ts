import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';

@Component
export default class EditSpecialDateComponent extends Vue {
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
	loading: boolean = false;
	failed: boolean = false;

	mounted() {
		this.loading = true;
		fetch('api/Admin/GetSpecialDateById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<SpecialDate>)
			.then(data => {
				this.specialdate = data;
				this.formatDate();
				this.loading = false;
			});
	}

	editSpecialDate() {
		this.failed = false;
		if (this.$refs.form.validate()) {
			fetch('api/Admin/UpdateSpecialDate', {
				method: 'PUT',
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

	cancel() {
		this.$router.push('/fetchadmin');
	}
}