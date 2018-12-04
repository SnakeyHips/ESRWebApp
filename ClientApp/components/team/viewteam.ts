import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { TeamSite } from '../../models/teamsite';
import { Team } from '../../models/team';

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

	team: Team = {
		id: 0,
		name: "",
		sV1Id: 0,
		sV1Name: "",
		drI1Id: 0,
		drI1Name: "",
		drI2Id: 0,
		drI2Name: "",
		rN1Id: 0,
		rN1Name: "",
		rN2Id: 0,
		rN2Name: "",
		rN3Id: 0,
		rN3Name: "",
		ccA1Id: 0,
		ccA1Name: "",
		ccA2Id: 0,
		ccA2Name: "",
		ccA3Id: 0,
		ccA3Name: "",
	}

	mounted() {
		fetch('api/Team/GetById?id=' + this.$route.params.id)
			.then(response => response.json() as Promise<Team>)
			.then(data => {
				this.team = data;
				this.headers = [
					{ text: 'Date', value: 'date' },
					{ text: 'Day', value: 'day' },
					{ text: this.team.sV1Name, value: 'sV1Site' },
					{ text: this.team.drI1Name, value: 'drI1Site' },
					{ text: this.team.drI2Name, value: 'drI2Site' },
					{ text: this.team.rN1Name, value: 'rN1Site' },
					{ text: this.team.rN2Name, value: 'rN2Site' },
					{ text: this.team.rN3Name, value: 'rN3Site' },
					{ text: this.team.ccA1Name, value: 'ccA1Site' },
					{ text: this.team.ccA2Name, value: 'ccA2Site' },
					{ text: this.team.ccA3Name, value: 'ccA3Site' },
				];
			})
	}

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
						this.loading = false;
					});
			} else {
				this.teamsites = [];
				this.failed = true;
			}
		}
	}

	statusColour(type: string) {
		switch (type) {
			case "Day Off":
				return "LightGray";
			case "Day Off - Part":
				return "LightGray";
			case "Annual Leave":
				return "Plum";
			case "Annual Leave - Part":
				return "Plum";
			case "Sick Leave":
				return "LightSeaGreen";
			case "Sick Leave - Part":
				return "LightSeaGreen";
			case "Special Leave":
				return "LightCoral";
			case "Special Leave - Part":
				return "LightCoral";
			case "Training":
				return "CornflowerBlue";
			case "Training - Part":
				return "CornflowerBlue";
		}
	}
	
	dateFormat(date: string) {
		return new Date(date).toLocaleDateString();
	}
}
