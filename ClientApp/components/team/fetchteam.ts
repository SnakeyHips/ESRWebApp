import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Team } from '../../models/team';

@Component
export default class FetchTeamComponent extends Vue {
	teams: Team[] = [];
	date: string = "";
	loading: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Name', value: 'name' },
		{ text: 'SV1', value: 'sV1Name' },
		{ text: 'DRI1', value: 'drI1Name' },
		{ text: 'DRI2', value: 'drI2Name' },
		{ text: 'RN1', value: 'rN1Name' },
		{ text: 'RN2', value: 'rN2Name' },
		{ text: 'RN3', value: 'rN3Name' },
		{ text: 'CCA1', value: 'ccA1Name' },
		{ text: 'CCA2', value: 'ccA2Name' },
		{ text: 'CCA3', value: 'ccA3Name' },
	];

	mounted() {
		this.loadTeams();
	}

	loadTeams() {
		this.loading = true;
		fetch('api/Team/GetTeams')
			.then(response => response.json() as Promise<Team[]>)
			.then(data => {
				this.teams = data;
				this.loading = false;
			});
	}

	createTeam() {
		this.$router.push("/createteam");
	}

	editTeam(id: number) {
		this.$router.push("/editteam/" + id);
	}

	viewTeam(id: number) {
		this.$router.push("/viewteam/" + id);
	}

	deleteTeam(id: number) {
		var ans = confirm("Do you want to delete this Team?");
		if (ans) {
			fetch('api/Team/Team?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete team. Please make sure you are still connected.");
					} else {
						this.loadTeams();
					}
				})
		}
	}
}
