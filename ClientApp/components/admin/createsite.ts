import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Site } from '../../models/site';

@Component
export default class CreateSiteComponent extends Vue {	

	site: Site = {
		id: 0,
		name: "",
		type: "",
		times: ""
	}

	createSite() {
		fetch('api/Admin/CreateSite', {
			method: 'POST',
			body: JSON.stringify(this.site)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to create Site. Please make sure all of the fields are completed.");
				} else {
					this.$router.push('/fetchadmin');
				}
			})
	}
}