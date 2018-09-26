import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { SelectedDate } from '../../models/selecteddate';

@Component
export default class FetchSessionComponent extends Vue {
	@Prop(SelectedDate) selecteddate!: SelectedDate;
	sessions: Session[] = [];
	date: string = "";
	loading: boolean = false;
	notedialog: boolean = false;
	search: string = "";
	headers: object[] = [
		{ text: 'Site', value: 'site' },
		{ text: 'Time', value: 'time' },
		{ text: 'Type', value: 'type' },
		{ text: 'LOD', value: 'lod' },
		{ text: 'Chairs', value: 'chairs' },
		{ text: 'OCC', value: 'occ' },
		{ text: 'Estimate', value: 'estimate' },
		{ text: 'Staff Count', value: 'staffCount' },
	];

	mounted() {
		this.loadSessions();
	}

	loadSessions() {
		this.loading = true;
		fetch('api/Session/GetSessions?date=' + this.selecteddate.date)
			.then(response => response.json() as Promise<Session[]>)
			.then(data => {
				this.sessions = data;
				this.loading = false;
			});
	}
	
	stateColour(state: number) {
		switch (state) {
			case 0:
				return 'gray';
			case 1:
				return 'black';
			case 2:
				return 'red';
		}
	}

	createSession() {
		this.$router.push("/createsession");
	}

	rosterSession(id: number) {
		this.$router.push("/rostersession/" + id);
	}

	editSession(session: Session) {
		if (session.sV1Id === 0 && session.drI1Id === 0 && session.drI2Id === 0 &&
			session.rN1Id === 0 && session.rN2Id === 0 && session.rN3Id === 0 &&
			session.ccA1Id === 0 && session.ccA2Id === 0 && session.ccA3Id === 0) {
			this.$router.push("/editsession/" + session.id);
		} else {
			alert("Please unroster staff before editing session!");
		}
	}

	overview() {
		let overview = this.$router.resolve({ path: "/overviewsession" });
		window.open(overview.href, '_blank');
	}

	deleteSession(session: Session) {
		if (session.sV1Id === 0 && session.drI1Id === 0 && session.drI2Id === 0 &&
			session.rN1Id === 0 && session.rN2Id === 0 && session.rN3Id === 0 &&
			session.ccA1Id === 0 && session.ccA2Id === 0 && session.ccA3Id === 0) {
			var ans = confirm("Do you want to delete this Session?");
			if (ans) {
				fetch('api/Session/Delete?id=' + session.id, {
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
		} else {
			alert("Please unroster staff before deleting session!");
		}
	}
}
