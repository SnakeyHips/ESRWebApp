import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { SpecialDate } from '../../models/specialdate';

@Component
export default class CreateSpecialDateComponent extends Vue {	

	specialdate: SpecialDate = {
		id: 0,
		name: "",
		date: ""
	}

	createSpecialDate() {
		fetch('api/Admin/CreateSpecialDate', {
			method: 'POST',
			body: JSON.stringify(this.specialdate)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to create Special Date. Please make sure all of the fields are completed.");
				} else {
					this.$router.push('/fetchadmin');
				}
			})
	}
}