<template>
  <div>
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-4">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
      {{ error }}
    </div>

    <!-- Detale -->
    <div v-else-if="agent" class="space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <h1 class="text-2xl font-semibold text-gray-900">Szczegóły agenta</h1>
        <div class="flex space-x-4">
          <button @click="editMode = true"
                  class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md">
            Edytuj
          </button>
          <router-link to="/agents"
                       class="text-blue-600 hover:text-blue-800">
            Wróc do listy
          </router-link>
        </div>
      </div>

      <!-- Infokarta -->
      <div class="bg-white shadow rounded-lg p-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <h2 class="text-lg font-medium text-gray-900 mb-4">Dane podstawowe</h2>
            <div class="space-y-4">
              <div>
                <span class="text-sm font-medium text-gray-500">Imię i nazwisko</span>
                <p class="mt-1">{{ agent.firstName }} {{ agent.lastName }}</p>
              </div>
              <div>
                <span class="text-sm font-medium text-gray-500">Email</span>
                <p class="mt-1">{{ agent.email }}</p>
              </div>
              <div>
                <span class="text-sm font-medium text-gray-500">Username</span>
                <p class="mt-1">{{ agent.username }}</p>
              </div>
            </div>
          </div>
          <div>
            <h2 class="text-lg font-medium text-gray-900 mb-4">Pozostałe</h2>
            <div class="space-y-4">
              <div>
                <span class="text-sm font-medium text-gray-500">Licencja</span>
                <p class="mt-1">{{ agent.licence || 'Brak' }}</p>
              </div>
     
              <div>
                <span class="text-sm font-medium text-gray-500">Status</span>
                <p class="mt-1">
                  <span class="px-2 py-1 text-sm rounded-full"
                        :class="agent.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                    {{ agent.isActive ? 'Aktywny' : 'Nieaktywny' }}
                  </span>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Raporty agenta -->
      <div class="bg-white shadow rounded-lg overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-lg font-medium text-gray-900">Raporty powiązane</h2>
        </div>

        <!-- Reports Table -->
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Typ</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Data złożenia</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Klient</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Akcje</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-if="reports.length === 0">
                <td colspan="6" class="px-6 py-4 text-center text-sm text-gray-500">
                  Brak raportów do pokazania
                </td>
              </tr>
              <tr v-for="report in reports" :key="report.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ report.id }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ report.insuranceType.name }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                        :class="{
                      'bg-yellow-100 text-yellow-800': report.status === 'Nowy',
                      'bg-blue-100 text-blue-800': report.status === 'WTrakcie',
                      'bg-green-100 text-green-800': report.status === 'Zaakceptowany',
                      'bg-gray-100 text-gray-800': report.status === 'Zamknięty'
                    }">
                    {{ report.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ new Date(report.submissionDate).toLocaleDateString() }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{
 report.endUser ?
                    `${report.endUser.firstName} ${report.endUser.lastName}` :
                    'Nieprzypisany'
                  }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <router-link :to="'/reports/' + report.id"
                               class="text-blue-600 hover:text-blue-900">
                    Wyświetl
                  </router-link>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <EditAgentModal v-if="editMode"
                   :agent="agent"
                   @close="editMode = false"
                   @update="handleUpdate" />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { agentsService } from '@/services/agentsService';
import EditAgentModal from '@/components/EditAgentModal.vue';

export default {
  name: 'AgentDetails',
  components: {
    EditAgentModal
  },
  setup() {
    const route = useRoute();
    const agent = ref(null);
    const reports = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const editMode = ref(false);

    const loadAgent = async () => {
      try {
        loading.value = true;
        error.value = null;

        const [agentResponse, reportsResponse] = await Promise.all([
          agentsService.getAgentById(route.params.id),
          agentsService.getAgentReports(route.params.id)
        ]);

        if (agentResponse.success && reportsResponse.success) {
          agent.value = agentResponse.data;
          reports.value = reportsResponse.data;
        } else {
          error.value = agentResponse.message || reportsResponse.message;
        }
      } catch (err) {
        error.value = 'Błąd ładowania szczegółów użytkownika.';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    const handleUpdate = (updatedAgent) => {
      agent.value = updatedAgent;
    };

    onMounted(() => {
      loadAgent();
    });

    return {
      agent,
      reports,
      loading,
      error,
      editMode,
      handleUpdate
    };
  }
};
</script>
