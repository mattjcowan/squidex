/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { slideAnimation, slideRightAnimation } from '@app/framework/internal';
import { LocalStoreService } from '../services/local-store.service';

@Component({
    selector: 'sqx-layout',
    styleUrls: ['./layout.component.scss'],
    templateUrl: './layout.component.html',
    animations: [
        slideAnimation,
        slideRightAnimation
    ],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class LayoutComponent implements OnInit {
    @Input()
    public name: string;

    @Input()
    public mode: 'blank' | 'left' | 'right' = 'blank';

    @Input()
    public isOpen = false;

    @Input()
    public maxWidth: string;

    @Input()
    public minWidth: string;

    @Input()
    public sidebarWidth: string;

    @ViewChild('panel', { static: false })
    public panel: ElementRef<HTMLElement>;

    constructor(
        private readonly localStore: LocalStoreService
    ) {
    }

    public ngOnInit() {
        if (this.name) {
            this.isOpen = this.localStore.getBoolean(`panel.${this.name}`);
        }
    }

    public toggle() {
        this.isOpen = !this.isOpen;

        if (this.name) {
            this.localStore.setBoolean(`panel.${this.name}`, this.isOpen);
        }
    }

    public padding(mode: string) {
        if (this.mode === mode) {
            if (this.isOpen) {
                return this.sidebarWidth;
            } else {
                return '5rem';
            }
        } else {
            return '';
        }
    }
}