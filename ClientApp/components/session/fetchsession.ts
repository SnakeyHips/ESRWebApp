import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';

@Component
export default class FetchSessionComponent extends Vue {
	sessions: Session[] = [];
	date: string = "";
	mount: boolean = true;

	mounted() {
		this.date = new Date().toISOString().slice(0, 10);
		this.loadSessions(this.date);
	}

	loadSessions(date: string) {
		this.mount = false;
		fetch('api/Session/GetSessions?date=' + date)
			.then(response => response.json() as Promise<Session[]>)
			.then(data => {
				this.sessions = data;
				this.mount = true;
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
