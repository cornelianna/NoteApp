document.getElementById('findFriendsButton').addEventListener('click', function() {
    var searchBar = document.getElementById('searchBar');
    if (searchBar.style.display === 'none') {
        searchBar.style.display = 'block';
    } else {
        searchBar.style.display = 'none';
    }
});

document.getElementById('searchInput').addEventListener('input', function() {
    var query = this.value;
    if (query.length > 0) {
        fetch('/Friends/SearchUsers?query=' + query)
            .then(response => response.json())
            .then(data => {
                var searchResults = document.getElementById('searchResults');
                searchResults.innerHTML = '';
                data.forEach(user => {
                    var li = document.createElement('li');
                    li.className = 'list-group-item';
                    li.textContent = user.userName + ' (' + user.email + ')';
                    li.addEventListener('click', function() {
                        addFriend(user.id);
                    });
                    li.addEventListener('mouseover', function() {
                        li.classList.add('active');
                    });
                    li.addEventListener('mouseout', function() {
                        li.classList.remove('active');
                    });
                    searchResults.appendChild(li);
                });
                searchResults.style.display = 'block';
            });
    } else {
        document.getElementById('searchResults').style.display = 'none';
    }
});

function addFriend(friendId) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    fetch('/Friends/AddFriend', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify({ FriendId: friendId })
    }).then(response => response.json())
      .then(data => {
          var feedbackMessage = document.getElementById('feedbackMessage');
          feedbackMessage.textContent = data.message;
          feedbackMessage.style.display = 'block';
          if (data.success) {
              feedbackMessage.className = 'alert alert-success';

              // Add the new friend to the friends table
              var friendsTable = document.getElementById('friendsTable');
              var tr = document.createElement('tr');
              var tdUsername = document.createElement('td');
              tdUsername.textContent = data.friend.userName;
              var tdEmail = document.createElement('td');
              tdEmail.textContent = data.friend.email;
              var tdActions = document.createElement('td');
              var deleteButton = document.createElement('button');
              deleteButton.className = 'btn btn-danger delete-friend-btn';
              deleteButton.textContent = 'Delete';
              deleteButton.setAttribute('data-friend-id', data.friend.id);
              deleteButton.style.display = 'none';
              deleteButton.addEventListener('click', function() {
                  deleteFriend(data.friend.id);
              });
              tdActions.appendChild(deleteButton);
              tr.appendChild(tdUsername);
              tr.appendChild(tdEmail);
              tr.appendChild(tdActions);
              friendsTable.appendChild(tr);

              // Add hover event listeners for the new row
              tr.addEventListener('mouseover', function() {
                  deleteButton.style.display = 'inline-block';
              });
              tr.addEventListener('mouseout', function() {
                  deleteButton.style.display = 'none';
              });
          } else {
              feedbackMessage.className = 'alert alert-danger';
          }
          setTimeout(() => {
              feedbackMessage.style.display = 'none';
          }, 3000);
      }).catch(error => {
          var feedbackMessage = document.getElementById('feedbackMessage');
          feedbackMessage.textContent = 'An error occurred while adding the friend.';
          feedbackMessage.className = 'alert alert-danger';
          feedbackMessage.style.display = 'block';
          setTimeout(() => {
              feedbackMessage.style.display = 'none';
          }, 3000);
      });
}

function deleteFriend(friendId) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    fetch('/Friends/DeleteFriend', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify({ FriendId: friendId })
    }).then(function(response) {
        if (response.ok) {
            location.reload();
        } else {
            alert('Failed to delete friend.');
        }
    });
}

function filterFriends() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("friendSearchInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("friendsTable");
    tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[0];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}

document.querySelectorAll('#friendsTable tr').forEach(function(row) {
    row.addEventListener('mouseover', function() {
        this.querySelector('.delete-friend-btn').style.display = 'inline-block';
    });
    row.addEventListener('mouseout', function() {
        this.querySelector('.delete-friend-btn').style.display = 'none';
    });
});

document.querySelectorAll('.delete-friend-btn').forEach(function(button) {
    button.addEventListener('click', function() {
        var friendId = this.getAttribute('data-friend-id');
        if (confirm('Are you sure you want to delete this friend?')) {
            deleteFriend(friendId);
        }
    });
});