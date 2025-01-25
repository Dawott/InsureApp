<!-- src/views/reports/ReportsList.vue -->
<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-semibold text-gray-900">Zgłoszenia Incydentów</h1>
      <router-link to="/reports/new"
                   class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md">
        Create New Report
      </router-link>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="text-center py-4">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
      {{ error }}
    </div>

    <!-- Tabelka -->
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Klient</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Typ</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Data złożenia</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Agent</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Akcje</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="report in reports" :key="report.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ report.id }}</td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ report.endUser.firstName }} {{ report.endUser.lastName }}</div>
              <div class="text-sm text-gray-500">{{ report.endUser.email }}</div>
            </td>
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
 report.insuranceAgent ?
                `${report.insuranceAgent.firstName} ${report.insuranceAgent.lastName}` :
                'Nieprzypisane'
              }}
            </td>
            <!--Wyświetl i skasuj -->
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <div class="flex space-x-2">
                <router-link :to="'/reports/' + report.id"
                             class="text-blue-600 hover:text-blue-900">
                  Wyświetl
                </router-link>
                <button @click="confirmDelete(report)"
                        class="text-red-600 hover:text-red-900">
                  Skasuj
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Delete -->
    <div v-if="showDeleteModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center">
      <div class="bg-white p-6 rounded-lg shadow-xl max-w-md w-full">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Usuń</h3>
        <p class="text-sm text-gray-500 mb-4">
          Czy chcesz skasować ten raport? Akcji nie można cofnąć!
        </p>
        <div class="flex justify-end space-x-3">
          <button @click="showDeleteModal = false"
                  class="bg-gray-100 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-200">
            Cancel
          </button>
          <button @click="deleteReport"
                  class="bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700">
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { reportsService } from '@/services/reportsService';

export default {
  name: 'ReportsList',
  setup() {
    const reports = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const showDeleteModal = ref(false);
    const reportToDelete = ref(null);

    const loadReports = async () => {
      try {
        loading.value = true;
        error.value = null;
        const response = await reportsService.getAllReports();
        if (response.success) {
          reports.value = response.data;
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd ładowania raportu. Spróbuj ponownie';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    const confirmDelete = (report) => {
      reportToDelete.value = report;
      showDeleteModal.value = true;
    };

    const deleteReport = async () => {
      if (!reportToDelete.value) return;

      try {
        const response = await reportsService.deleteReport(reportToDelete.value.id);
        if (response.success) {
          reports.value = reports.value.filter(r => r.id !== reportToDelete.value.id);
          showDeleteModal.value = false;
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd przy kasowania raportu. Spróbuj ponownie';
        console.error('Błąd:', err);
      }
    };

    onMounted(() => {
      loadReports();
    });

    return {
      reports,
      loading,
      error,
      showDeleteModal,
      confirmDelete,
      deleteReport
    };
  }
};
</script>
