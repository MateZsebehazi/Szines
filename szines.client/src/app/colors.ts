import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class Colors {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getColors() {
    return this.http.get<any[]>(this.apiUrl);
  }

  addColor(color: any) {
    return this.http.post(this.apiUrl, color);
  }

  deleteColor(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

}
