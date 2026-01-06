import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Colors } from '../colors';
import { Color } from '../color';

@Component({
  selector: 'app-color-manager',
  standalone: false,
  templateUrl: './color-manager.html',
  styleUrl: './color-manager.scss',
})
export class ColorManager implements OnInit, OnDestroy {
  colors: Color[] = [];
  showAddForm = false;
  newColor: Color = {
    name: '',
    hexValue: '#000000'
  };
  private pollingInterval?: number;

  constructor(private colorService: Colors) { }

  ngOnInit() {
    this.loadColors();
    this.startPolling();
  }

  ngOnDestroy() {
    this.stopPolling();
  }

  private startPolling() {
    this.pollingInterval = window.setInterval(() => {
      this.loadColors();
    }, 5000);
  }

  private stopPolling() {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
    }
  }

  loadColors() {
    this.colorService.getColors().subscribe({
      next: (data) => this.colors = data,
      error: (err) => console.error('Error loading colors:', err)
    });
  }

  deleteColor(id: number | undefined) {
    if (!id) return;
    this.colorService.deleteColor(id).subscribe({
      next: () => this.loadColors(),
      error: (err) => console.error('Error deleting color:', err)
    });
  }

  toggleAddForm() {
    this.showAddForm = !this.showAddForm;
  }

  addColor() {
    if (this.newColor.name && this.newColor.hexValue) {
      this.colorService.addColor(this.newColor).subscribe({
        next: () => {
          this.loadColors();
          this.newColor = { name: '', hexValue: '#000000' };
          this.showAddForm = false;
        },
        error: (err) => console.error('Error adding color:', err)
      });
    }
  }

  copyToClipboard(hex: string) {
    navigator.clipboard.writeText(hex).then(() => {
      alert('Copied: ' + hex);
    });
  }
}
