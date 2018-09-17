import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Site } from '../../models/site';

@Component
export default class EditSiteComponent extends Vue {

	mount: boolean = false;
	site: Site = {
		id: 0,
		name: "",
		type: "",
		times: "",
	}

	mounted() {
		fetch('api/Admin/GetSiteById?id=' + this.$route.params.id)
			.then(respone => respone.json() as Promise<Site>)
			.then(data => {
				this.site = data;
				this.mount = true;
			});
	}

	editSite() {
		fetch('api/Admin/UpdateSite', {
			method: 'PUT',
			body: JSON.stringify(this.site)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to edit Site. Please make sure all fields are correct.");
				} else {
					this.$router.push('/fetchadmin');
				}
			})
	}
}