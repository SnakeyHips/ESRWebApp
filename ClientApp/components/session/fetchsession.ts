import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';

@Component
export default class FetchSessionComponent extends Vue {
	sessions: Session[] = [];
	date: string = "";
	loading: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Date', value: 'date'	},
		{ text: 'Type', value: 'type' },
		{ text: 'Site', value: 'site' },
		{ text: 'Time', value: 'time' },
		{ text: 'LOD', value: 'lod' },
		{ text: 'Chairs', value: 'chairs' },
		{ text: 'OCC', value: 'occ' },
		{ text: 'Estimate', value: 'estimate' },
		{ text: 'Staff Count', value: 'staffCount' },
	];

	loadSessions(date: string) {
		this.loading = true;
		fetch('api/Session/GetSessions?date=' + date)
			.then(response => response.json() as Promise<Session[]>)
			.then(data => {
				this.sessions = data;
				this.loading = false;
			});
	}

	createSession() {
		this.$router.push("/createsession");
	}

	rosterSession(id: number) {
		this.$router.push("/rostersession/" + id);
	}

	editSession(id: number) {
		this.$router.push("/editsession/" + id);
	}

	overview() {
		let overview = this.$router.resolve({ path: "/overviewsession" });
		window.open(overview.href, '_blank');
	}

	deleteSession(id: number) {
		var ans = confirm("Do you want to delete this Session?" + id);
		if (ans) {
			fetch('api/Session/Delete?id=' + id, {
				method: 'DELETE'
			})
				.then(response => response.json() as Promise<number>)
				.then(data => {
					if (data < 1) {
						alert("Failed to delete Session. Please make sure you are still connected.");
					} else {
						this.loadSessions(this.date);
					}
				})
		}
	}
}
