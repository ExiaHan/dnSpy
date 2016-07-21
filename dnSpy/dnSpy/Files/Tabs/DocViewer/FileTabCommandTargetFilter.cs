﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using dnSpy.Contracts.Command;
using Microsoft.VisualStudio.Text.Editor;

namespace dnSpy.Files.Tabs.DocViewer {
	sealed class FileTabCommandTargetFilter : ICommandTargetFilter {
		readonly ITextView textView;

		public FileTabCommandTargetFilter(ITextView textView) {
			this.textView = textView;
		}

		DocumentViewerControl TryGetInstance() =>
			__documentViewerControl ?? (__documentViewerControl = DocumentViewerControl.TryGetInstance(textView));
		DocumentViewerControl __documentViewerControl;

		public CommandTargetStatus CanExecute(Guid group, int cmdId) {
			if (TryGetInstance() == null)
				return CommandTargetStatus.NotHandled;

			if (group == FileTabCommandConstants.FileTabGroup) {
				switch ((FileTabIds)cmdId) {
				case FileTabIds.MoveToNextReference:
				case FileTabIds.MoveToPreviousReference:
				case FileTabIds.MoveToNextDefinition:
				case FileTabIds.MoveToPreviousDefinition:
				case FileTabIds.FollowReference:
				case FileTabIds.FollowReferenceNewTab:
				case FileTabIds.ClearMarkedReferencesAndToolTip:
					return CommandTargetStatus.Handled;

				default:
					Debug.Fail($"Unknown {nameof(FileTabIds)} id: {(FileTabIds)cmdId}");
					return CommandTargetStatus.NotHandled;
				}
			}
			return CommandTargetStatus.NotHandled;
		}

		public CommandTargetStatus Execute(Guid group, int cmdId, object args = null) {
			object result = null;
			return Execute(group, cmdId, args, ref result);
		}

		public CommandTargetStatus Execute(Guid group, int cmdId, object args, ref object result) {
			var textCtrl = TryGetInstance();
			if (textCtrl == null)
				return CommandTargetStatus.NotHandled;

			if (group == FileTabCommandConstants.FileTabGroup) {
				switch ((FileTabIds)cmdId) {
				case FileTabIds.MoveToNextReference:
					textCtrl.MoveReference(true);
					return CommandTargetStatus.Handled;

				case FileTabIds.MoveToPreviousReference:
					textCtrl.MoveReference(false);
					return CommandTargetStatus.Handled;

				case FileTabIds.MoveToNextDefinition:
					textCtrl.MoveToNextDefinition(true);
					return CommandTargetStatus.Handled;

				case FileTabIds.MoveToPreviousDefinition:
					textCtrl.MoveToNextDefinition(false);
					return CommandTargetStatus.Handled;

				case FileTabIds.FollowReference:
					textCtrl.FollowReference();
					return CommandTargetStatus.Handled;

				case FileTabIds.FollowReferenceNewTab:
					textCtrl.FollowReferenceNewTab();
					return CommandTargetStatus.Handled;

				case FileTabIds.ClearMarkedReferencesAndToolTip:
					//TODO:textCtrl.ClearMarkedReferencesAndToolTip();
					return CommandTargetStatus.Handled;

				default:
					Debug.Fail($"Unknown {nameof(FileTabIds)} id: {(FileTabIds)cmdId}");
					return CommandTargetStatus.NotHandled;
				}
			}
			return CommandTargetStatus.NotHandled;
		}

		public void SetNextCommandTarget(ICommandTarget commandTarget) { }
		public void Dispose() { }
	}
}