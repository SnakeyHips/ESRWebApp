import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Session } from '../../models/session';

@Component
export default class CreateSessionComponent extends Vue {	

	session: Session = {
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

	createSession() {
		fetch('api/Session/Create', {
			method: 'POST',
			body: JSON.stringify(this.session)
		})
			.then(response => response.json() as Promise<number>)
			.then(data => {
				if (data < 1) {
					alert("Failed to create Session. Please make sure there is not already a session of that same date, time and location.");
				} else {
					this.$router.push('/fetchsession');
				}
			})
	}
}