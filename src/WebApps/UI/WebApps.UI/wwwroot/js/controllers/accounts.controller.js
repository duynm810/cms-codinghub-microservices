const accountsController = function () {
    this.initialize = function () {

        this.uploadFileHandlers();

        // Auto-generate slug from title
        document.querySelector('input[name="Title"]').addEventListener('input', function () {
            let titleValue = this.value;
            document.querySelector('input[name="Slug"]').value = createSlug(titleValue);
        });
    }

    this.uploadFileHandlers = function () {
        const fileInput = document.getElementById("thumbnail");
        const dropArea = document.getElementById("drop-area");
        const fileList = document.getElementById("file-list");

        if (!fileInput || !dropArea || !fileList) 
            return;

        dropArea.addEventListener("dragover", (event) => {
            event.preventDefault();
            dropArea.classList.add("drag-over");
        });

        dropArea.addEventListener("dragleave", () => {
            dropArea.classList.remove("drag-over");
        });

        dropArea.addEventListener("drop", async (event) => {
            event.preventDefault();
            dropArea.classList.remove("drag-over");
            await this.handleFiles(event.dataTransfer.files);
        });

        fileInput.addEventListener("change", async () => {
            await this.handleFiles(fileInput.files);
        });

        this.handleFiles = async (files) => {
            const fileList = document.getElementById("file-list");
            
            fileList.innerHTML = '';
            [...files].forEach(file => {
                const fileItem = document.createElement("div");
                fileItem.className = "file-item";

                const fileThumbnail = document.createElement("div");
                fileThumbnail.className = "file-thumbnail";
                const img = document.createElement("img");
                img.src = URL.createObjectURL(file);
                img.alt = "Thumbnail";
                fileThumbnail.appendChild(img);

                const fileDetails = document.createElement("div");
                fileDetails.className = "file-details";
                fileDetails.innerHTML = `
                    <span class="file-name">${file.name}</span>
                    <span class="file-size">${(file.size / 1024 / 1024).toFixed(2)} MB</span>
                `;

                const fileRemove = document.createElement("div");
                fileRemove.className = "file-remove";
                const removeButton = document.createElement("button");
                removeButton.type = "button";
                removeButton.innerHTML = '<i class="fas fa-trash-alt"></i>';
                removeButton.addEventListener("click", () => {
                    showConfirmAlert(
                        'Are you sure you want to delete this image?',
                        'This action cannot be undone.',
                        'Yes, delete it',
                        'Cancel',
                        async () => {
                            const thumbnailUrl = document.getElementById('thumbnailUrl').value;
                            if (thumbnailUrl) {
                                const deleteSuccess = await this.deleteImage(thumbnailUrl);
                                if (deleteSuccess) {
                                    const postId = document.getElementById('postId').value;
                                    if (postId) {
                                        await this.updatePostThumbnail(postId);
                                    }
                                    fileInput.value = '';
                                    fileList.innerHTML = '';
                                    document.getElementById('thumbnailUrl').value = ''; // Xóa URL hình ảnh khi xóa file
                                } else {
                                    this.showErrorNotification('Failed to delete image.');
                                }
                            }
                        }
                    );
                });

                fileRemove.appendChild(removeButton);
                fileItem.appendChild(fileThumbnail);
                fileItem.appendChild(fileDetails);
                fileItem.appendChild(fileRemove);
                fileList.appendChild(fileItem);
            });

            await this.uploadFiles(files);
        }

        this.uploadFiles = async (files) => {
            const formData = new FormData();
            formData.append('file', files[0]);
            formData.append('type', 'posts');

            try {
                const response = await fetch('/media/upload-image', {
                    method: 'POST',
                    body: formData
                });

                const responseBody = await response.text();
                if (!response.ok) {
                    showErrorNotification(`Upload failed with status: ${response.status}, error: ${responseBody}`);
                    return;
                }

                const result = JSON.parse(responseBody);
                console.log(result);

                // Update the hidden input with the URL of the uploaded file
                document.getElementById('thumbnailUrl').value = result.data;
            } catch (error) {
                showErrorNotification(`Error uploading file: ${error.message}`);
            }
        }

        this.deleteImage = async (imagePath) => {
            try {
                const response = await fetch(`/media/delete-image/${encodeURIComponent(imagePath)}`, {
                    method: 'DELETE'
                });

                if (!response.ok) {
                    showErrorNotification(`Delete failed with status: ${response.status}`);
                    return false;
                }

                const result = await response.json();
                console.log(`Image deleted successfully: ${imagePath}`);
                return result.isSuccess && result.data;
            } catch (error) {
                showErrorNotification(`Error deleting file: ${error.message}`);
                return false;
            }
        }

        this.updatePostThumbnail = async (postId) => {
            try {
                console.log(`Updating thumbnail for post with ID: ${postId}`);
                const response = await fetch(`/posts/update-thumbnail/${postId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        thumbnail: null // Set the thumbnail to null
                    })
                });

                if (!response.ok) {
                    showErrorNotification(`Post update failed with status: ${response.status}`);
                    return;
                }

                const result = await response.json();
                console.log('Post thumbnail updated successfully');
            } catch (error) {
                showErrorNotification(`Error updating post: ${error.message}`);
            }
        }

        this.waitForUploadCompletion = async () => {
            const thumbnailUrl = document.getElementById('thumbnailUrl');
            while (!thumbnailUrl.value) {
                await new Promise(resolve => setTimeout(resolve, 100));
            }
        }
    }
}