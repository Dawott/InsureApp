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

    <!-- Report Details -->
    <div v-else-if="report" class="space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <h1 class="text-2xl font-semibold text-gray-900">
          Szczegóły zgłoszenia #{{ report.id }}
        </h1>
        <div class="flex space-x-4">
          <button @click="editMode = true"
                  class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md">
            Edytuj
          </button>
          <router-link to="/reports"
                       class="text-blue-600 hover:text-blue-800">
            Powrót
          </router-link>
        </div>
        </div>

        <!-- Main Content -->
        <div class="bg-white shadow rounded-lg">
          <!-- Status Banner -->
          <div class="px-6 py-4 border-b border-gray-200">
            <span class="px-3 py-1 text-sm font-semibold rounded-full"
                  :class="{
              'bg-yellow-100 text-yellow-800': report.status === 'Nowy',
              'bg-blue-100 text-blue-800': report.status === 'WTrakcie',
              'bg-green-100 text-green-800': report.status === 'Zaakceptowany',
              'bg-gray-100 text-gray-800': report.status === 'Zamknięty'
            }">
              {{ report.status }}
            </span>
          </div>

          <!-- Raport -->
          <div class="px-6 py-4 grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Klient -->
            <div class="space-y-4">
              <h3 class="text-lg font-medium text-gray-900">Informacja o kliencie</h3>
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Klient</p>
                <p class="text-sm font-medium text-gray-900">
                  {{ report.endUser.firstName }} {{ report.endUser.lastName }}
                </p>
              </div>
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Email</p>
                <p class="text-sm font-medium text-gray-900">{{ report.endUser.email }}</p>
              </div>
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Telefon</p>
                <p class="text-sm font-medium text-gray-900">
                  {{ report.endUser.phoneNumber || 'Nie podano' }}
                </p>
              </div>
            </div>

            <!-- Szczegóły -->
            <div class="space-y-4">
              <h3 class="text-lg font-medium text-gray-900">Szczegóły raportu</h3>
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Kategoria</p>
                <p class="text-sm font-medium text-gray-900">
                  {{ report.insuranceType.name }}
                </p>
              </div>
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Data złożenia</p>
                <p class="text-sm font-medium text-gray-900">
                  {{ new Date(report.submissionDate).toLocaleDateString() }}
                </p>
              </div>
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Przypisany agent</p>
                <p class="text-sm font-medium text-gray-900">
                  {{
 report.insuranceAgent ?
                  `${report.insuranceAgent.firstName} ${report.insuranceAgent.lastName}` :
                  'Nieprzypisany'
                  }}
                </p>
              </div>
            </div>

            <!-- Opisy -->
            <div class="col-span-1 md:col-span-2 space-y-4">
              <div class="space-y-2">
                <p class="text-sm text-gray-600">Opis zdarzenia</p>
                <p class="text-sm text-gray-900">{{ report.description }}</p>
              </div>
              <div class="space-y-2" v-if="report.additionalNotes">
                <p class="text-sm text-gray-600">Dodatkowe notatki</p>
                <p class="text-sm text-gray-900">{{ report.additionalNotes }}</p>
              </div>
              <div class="space-y-2" v-if="report.decisionReason">
                <p class="text-sm text-gray-600">Powód decyzji</p>
                <p class="text-sm text-gray-900">{{ report.decisionReason }}</p>
              </div>
            </div>

            <!-- Doksy -->
            <div class="col-span-1 md:col-span-2" v-if="report.documents && report.documents.length > 0">
              <h3 class="text-lg font-medium text-gray-900 mb-4">Dokumentacja</h3>
              <ul class="divide-y divide-gray-200">
                <li v-for="doc in report.documents" :key="doc.id" class="py-3">
                  <div class="flex items-center justify-between">
                    <div>
                      <p class="text-sm font-medium text-gray-900">{{ doc.fileName }}</p>
                      <p class="text-sm text-gray-500">{{ new Date(doc.uploadDate).toLocaleDateString() }}</p>
                    </div>
                    <!-- Do zrobienia jeszcze download -->
                    <button class="text-blue-600 hover:text-blue-800 text-sm">
                      Ściągnij
                    </button>
                  </div>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
      <!-- Modal do edycji -->
    <EditReportModal v-if="editMode"
                     :report="report"
                     @close="editMode = false"
                     @update="handleUpdate" />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { reportsService } from '@/services/reportsService';
import EditReportModal from '@/components/EditReportModal.vue';
  //import EditReportModal from '../components/reports/EditReportModal.vue';

export default {
    name: 'ReportDetails',
    components: {
      EditReportModal
    },
  setup() {
    const route = useRoute();
    const report = ref(null);
    const loading = ref(true);
    const error = ref(null);
    const editMode = ref(false);

    const loadReport = async () => {
      try {
        loading.value = true;
        error.value = null;
        const response = await reportsService.getReportById(route.params.id);
        if (response.success) {
          report.value = response.data;
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

    onMounted(() => {
      loadReport();
    });
    const handleUpdate = (updatedReport) => {
      report.value = updatedReport;
    };

    return {
      report,
      loading,
      error,
      editMode,
      handleUpdate
    };
  }
};
</script>
