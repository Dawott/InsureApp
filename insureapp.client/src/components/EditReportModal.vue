<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center">
    <div class="bg-white rounded-lg shadow-xl max-w-4xl w-full m-4 max-h-[90vh] overflow-y-auto">
      <!-- Modal Header -->
      <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">Edytuj zgłoszenie #{{ report.id }}</h2>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-500">
            <span class="sr-only">Zamknij</span>
            <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="px-6 py-4">
        <div class="space-y-6">
          <!-- Status -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Status</label>
            <select v-model="formData.status"
                    class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500">
              <option value="Nowy">Nowy</option>
              <option value="WTrakcie">W trakcie</option>
              <option value="Zaakceptowany">Zaakceptowany</option>
              <option value="Zamknięty">Zamknięty</option>
            </select>
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Opis zdarzenia</label>
            <textarea v-model="formData.description"
                      rows="3"
                      class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"></textarea>
          </div>

          <!-- Additional Notes -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Dodatkowe notatki</label>
            <textarea v-model="formData.additionalNotes"
                      rows="2"
                      class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"></textarea>
          </div>

          <!-- Decision Reason -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Powód decyzji</label>
            <textarea v-model="formData.decisionReason"
                      rows="2"
                      class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"></textarea>
          </div>

          <!-- Insurance Agent -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Przypisany Agent</label>
            <select v-model="formData.insuranceAgentId"
                    class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500">
              <option :value="null">Nieprzypisane</option>
              <option v-for="agent in agents"
                      :key="agent.id"
                      :value="agent.id">
                {{ agent.firstName }} {{ agent.lastName }}
              </option>
            </select>
          </div>

          <!-- Error Message -->
          <div v-if="error" class="text-red-600 text-sm">
            {{ error }}
          </div>
        </div>

        <!-- Form Actions -->
        <div class="mt-6 flex justify-end space-x-3">
          <button type="button"
                  @click="$emit('close')"
                  class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            Anuluj
          </button>
          <button type="submit"
                  :disabled="loading"
                  class="inline-flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50">
            <span v-if="loading">Zapisywanie...</span>
            <span v-else>Zapisz zmiany</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { reportsService } from '@/services/reportsService';
import axios from 'axios';

export default {
  name: 'EditReportModal',
  props: {
    report: {
      type: Object,
      required: true
    }
  },
  emits: ['close', 'update'],
  setup(props, { emit }) {
    const formData = ref({
      id: props.report.id,
      status: props.report.status,
      description: props.report.description,
      additionalNotes: props.report.additionalNotes,
      decisionReason: props.report.decisionReason,
      insuranceAgentId: props.report.insuranceAgentId,
      insuranceTypeId: props.report.insuranceTypeId,
      endUserId: props.report.endUserId
    });

    const agents = ref([]);
    const loading = ref(false);
    const error = ref(null);

    const loadAgents = async () => {
      try {
        const response = await axios.get('api/InsuranceAgents/GetAllAgents');
        if (response.data.success) {
          agents.value = response.data.data;
        }
      } catch (err) {
        console.error('Błąd ładowania agentów:', err);
      }
    };

    const handleSubmit = async () => {
      try {
        loading.value = true;
        error.value = null;

        const response = await reportsService.updateReport(props.report.id, formData.value);

        if (response.success) {
          emit('update', response.data);
          emit('close');
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd ładowania raportów. Spróbuj ponownie';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    onMounted(() => {
      loadAgents();
    });

    return {
      formData,
      agents,
      loading,
      error,
      handleSubmit
    };
  }
};
</script>
