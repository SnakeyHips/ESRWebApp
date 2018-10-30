import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Session } from '../../models/session';
import { SelectedDate } from '../../models/selecteddate';

@Component
export default class FetchSessionComponent extends Vue {
	@Prop(SelectedDate) selecteddate!: SelectedDate;
	dateFormatted: string = new Date(this.selecteddate.date).toLocaleDateString();
	sessions: Session[] = [];
	loading: boolean = false;
	deletedialog: boolean = false;
	viewdialog: boolean = false;
	notedialog: boolean = false;
	failed: boolean = false;
	errorMessage: string = "";
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

	selected: Session = {
		id: 0,
		date: "",
		day: "",
		type: "",
		site: "",
		time: "",
		lod: 0,
		chairs: 0,
		occ: 0,
		estimate: 0,
		holiday: 0,
		note: "",
		sV1Id: 0,
		sV1Name: "",
		sV1LOD: 0,
		sV1UNS: 0,
		sV1OT: 0,
		drI1Id: 0,
		drI1Name: "",
		drI1LOD: 0,
		drI1UNS: 0,
		drI1OT: 0,
		drI2Id: 0,
		drI2Name: "",
		drI2LOD: 0,
		drI2UNS: 0,
		drI2OT: 0,
		rN1Id: 0,
		rN1Name: "",
		rN1LOD: 0,
		rN1UNS: 0,
		rN1OT: 0,
		rN2Id: 0,
		rN2Name: "",
		rN2LOD: 0,
		rN2UNS: 0,
		rN2OT: 0,
		rN3Id: 0,
		rN3Name: "",
		rN3LOD: 0,
		rN3UNS: 0,
		rN3OT: 0,
		ccA1Id: 0,
		ccA1Name: "",
		ccA1LOD: 0,
		ccA1UNS: 0,
		ccA1OT: 0,
		ccA2Id: 0,
		ccA2Name: "",
		ccA2LOD: 0,
		ccA2UNS: 0,
		ccA2OT: 0,
		ccA3Id: 0,
		ccA3Name: "",
		ccA3LOD: 0,
		ccA3UNS: 0,
		ccA3OT: 0,
		staffCount: 0,
		state: 0
	}

	mounted() {
		this.loadSessions();
	}

	loadSessions() {
		this.loading = true;
		this.dateFormatted = new Date(this.selecteddate.date).toLocaleDateString();
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
			case 2:
				return 'red';
		}
	}

	dateFormat() {
		return new Date(this.selecteddate.date).getDate().toString();
	}

	createSession() {
		this.$router.push("/createsession");
	}

	rosterSession(id: number) {
		this.$router.push("/rostersession/" + id);
	}

	editSession(selected: Session) {
		if (selected.sV1Id === 0 && selected.drI1Id === 0 && selected.drI2Id === 0 &&
			selected.rN1Id === 0 && selected.rN2Id === 0 && selected.rN3Id === 0 &&
			selected.ccA1Id === 0 && selected.ccA2Id === 0 && selected.ccA3Id === 0) {
			this.$router.push("/editsession/" + selected.id);
		} else {
			this.errorMessage = "Please unroster staff before editing session!";
			this.failed = true;
		}
	}

	openNote(selected: Session) {
		this.selected = selected;
		this.notedialog = true;
	}

	openView(selected: Session) {
		this.selected = selected;
		this.viewdialog = true;
	}

	openDelete(selected: Session) {
		if (selected.sV1Id === 0 && selected.drI1Id === 0 && selected.drI2Id === 0 &&
			selected.rN1Id === 0 && selected.rN2Id === 0 && selected.rN3Id === 0 &&
			selected.ccA1Id === 0 && selected.ccA2Id === 0 && selected.ccA3Id === 0) {
			this.selected = selected;
			this.deletedialog = true;
		} else {
			this.errorMessage = "Please unroster staff before deleting session!";
			this.failed = true;
		}
	}

	deleteSession() {
		this.failed = false;
		this.deletedialog = false;
		fetch('api/Session/Delete?id=' + this.selected.id, {
			method: 'DELETE'
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					this.errorMessage = "Failed to delete session!";
					this.failed = true;
				} else {
					this.loadSessions();
				}
			})
	}
}
