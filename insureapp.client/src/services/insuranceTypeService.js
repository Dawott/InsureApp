import axios from 'axios';

const API_URL = '/api/InsuranceTypes';

export const insuranceTypesService = {
  async getAllTypes() {
    const response = await axios.get(`${API_URL}/ShowAllTypes`);
    return response.data;
  },

  async getTypeById(id) {
    const response = await axios.get(`${API_URL}/GetTypeByID/${id}`);
    return response.data;
  },

  async createType(typeData) {
    const response = await axios.post(`${API_URL}/CreateType`, typeData);
    return response.data;
  },

  async updateType(id, typeData) {
    const response = await axios.put(`${API_URL}/EditType/${id}`, typeData);
    return response.data;
  },

  async deleteType(id) {
    const response = await axios.delete(`${API_URL}/DeleteType/${id}`);
    return response.data;
  }
};
