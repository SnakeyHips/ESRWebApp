import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';

@Component
export default class EditSpecialDateComponent extends Vue {

	mount: boolean = false;
	specialdate: SpecialDate = {
		id: 0,
		name: "",
		date: ""
	}

	mounted() {
		fetch('api/Admin/GetSpecialDateById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<SpecialDate>)
			.then(data => {
				this.specialdate = data;
				this.mount = true;
			});
	}

	editSpecialDate() {
		fetch('api/Admin/UpdateSpecialDate', {
			method: 'PUT',
			body: JSON.stringify(this.specialdate)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to edit Special Date. Please make sure all fields are correct.");
				} else {
					this.$router.push('/fetchadmin');
				}
			})
	}
}