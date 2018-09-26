import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { TeamSite } from '../../models/teamsite';

@Component
export default class ViewTeamComponent extends Vue {
	teamsites: TeamSite[] = [];
	startdate: string = "";
	enddate: string = "";
	startDateFormatted: string = "";
	endDateFormatted: string = "";
	loading: boolean = false;
	failed: boolean = false;
	search: string = "";
	headers: object[] = [];

	loadSessions() {
		if (this.startdate != "") {
			this.startDateFormatted = new Date(this.startdate).toLocaleDateString();
		}
		if (this.enddate != "") {
			this.endDateFormatted = new Date(this.enddate).toLocaleDateString();
		}
		if (this.startdate != "" && this.enddate != "") {
			if (this.enddate >= this.startdate) {
				this.loading = true;
				fetch('api/Team/GetTeamSites?id=' + this.$route.params.id + '&startdate=' + this.startdate + '&enddate=' + this.enddate)
					.then(response => response.json() as Promise<TeamSite[]>)
					.then(data => {
						this.failed = false;
						this.teamsites = data;
						this.headers = [
							{ text: 'Date', value: 'date' },
							{ text: 'Day', value: 'day' },
							{ text: this.teamsites[0].sV1Name, value: 'sV1Site' },
							{ text: this.teamsites[0].drI1Name, value: 'drI1Site' },
							{ text: this.teamsites[0].drI2Name, value: 'drI2Site' },
							{ text: this.teamsites[0].rN1Name, value: 'rN1Site' },
							{ text: this.teamsites[0].rN2Name, value: 'rN2Site' },
							{ text: this.teamsites[0].rN3Name, value: 'rN3Site' },
							{ text: this.teamsites[0].ccA1Name, value: 'ccA1Site' },
							{ text: this.teamsites[0].ccA2Name, value: 'ccA2Site' },
							{ text: this.teamsites[0].ccA3Name, value: 'ccA3Site' },
						];
						this.loading = false;
					});
			} else {
				this.teamsites = [];
				this.failed = true;
			}
		}
	}

	siteColour(type: string) {
		switch (type) {
			case "Day Off":
				return "LightGray";
			case "Annual Leave":
				return "Plum";
			case "Sick Leave":
				return "LightSeaGreen";
			case "Special Leave":
				return "LightCoral";
			case "Training":
				return "CornflowerBlue";
		}
	}
}
