<template>
	<v-container>
		<h1>Create Session</h1>
		<v-form ref="form" lazy-validation>
			<v-menu :nudge-right="40" lazy transition="scale-transition" offset-y >
				<v-text-field slot="activator" v-model="session.date" label="Session date" append-icon="event" :rules="[rules.required]" readonly required></v-text-field>
				<v-date-picker v-model="session.date"></v-date-picker>
			</v-menu>
			<v-select v-model="session.type" :items="types" label="Type" v-on:change="loadSites(session.type)" :rules="[rules.required]" requried />
			<v-select v-model="session.site" :items="sites" label="Site" name="session.site" item-text="name" v-on:change="loadTimes(session.site)" :rules="[rules.required]" requried />
			<v-select v-model="session.time" :items="times" label="Clinic Time" :rules="[rules.required]" requried />
			<v-text-field v-model="session.lod" label="LOD" :rules="[rules.required, rules.decimal]" maxlength="5" required />
			<v-text-field v-model="session.chairs" label="Chairs" :rules="[rules.required, rules.number]" maxlength="2" required />
			<v-text-field v-model="session.occ" label="OCC" :rules="[rules.required, rules.number]" maxlength="2" required />
			<v-text-field v-model="session.estimate" label="Estimate" :rules="[rules.required, rules.number]"maxlength="2"  required />
			<v-btn v-on:click="createSession">Submit</v-btn>
			<v-btn v-on:click="clear">Clear</v-btn>
			<v-btn v-on:click="cancel">Cancel</v-btn>
			<v-alert :value="failed" type="error">Failed to create session!</v-alert>
		</v-form>
	</v-container>
</template>

<script src="./createsession.ts"></script>
