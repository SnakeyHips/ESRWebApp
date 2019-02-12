import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { TeamSite } from '../../models/teamsite';
import { Team } from '../../models/team';
import { AbsenceType } from '../../models/absencetype';

@Component
export default class ViewTeamComponent extends Vue {
	teamsites: TeamSite[] = [];
	absencetypes: AbsenceType[] = [];
	startdate: string = "";
	enddate: string = "";
	startDateFormatted: string = "";
	endDateFormatted: string = "";
	loading: boolean = false;
	failed: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Date', value: 'date' },
		{ text: 'Day', value: 'day' },
	];

	team: Team = {
		id: 0,
		name: "",
		members: []
	}

	mounted() {
		fetch('api/Team/GetById?id=' + this.$route.params.id)
			.then(response => response.json() as Promise<Team>)
			.then(data => {
				this.team = data;
				for (var i = 0; i < this.team.members.length; i++) {
					this.headers.push({ text: this.team.members[i].employeeName, value: 'employeeSite' });
				}
			})
		this.loadAbsenceTypes();
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

	loadAbsenceTypes() {
		fetch('api/Admin/GetAbsenceTypes')
			.then(response => response.json() as Promise<AbsenceType[]>)
			.then(data => {
				this.absencetypes = data;
			});
	}

	siteColour(type: string) {
		for (var i = 0; i < this.absencetypes.length; i++) {
			if (type.includes(this.absencetypes[i].name)) {
				return this.absencetypes[i].colour;
			}
		}
	}

	dateFormat(date: string) {
		return new Date(date).toLocaleDateString();
	}
}
