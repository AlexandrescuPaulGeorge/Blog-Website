using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString()})
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,

            };
            var selectedTags = new List<Tag>();
            //map tags from selected Tags
            foreach(var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

                if(existingTag != null)
                {
                    selectedTags.Add(existingTag);  
                }
            }
            //Mapping tags back to domain model
            blogPost.Tags = selectedTags;
            await blogPostRepository.AddAsync(blogPost);
            return RedirectToAction("Add"); 
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {

            // Call the repo
            var blogPosts = await blogPostRepository.GetAllAsync();


            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Retrieve the result from the repo
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagsDomainModel = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {


                //map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),

                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };

                //pass data to view
                return View(model);
            }
            return View(null);
        }

        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            //map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                PublishedDate = editBlogPostRequest.PublishedDate,
                UrlHandle = editBlogPostRequest.UrlHandle,
                Visible = editBlogPostRequest.Visible
            };
            //Map Tags into domain model
            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if(Guid.TryParse(selectedTag,out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if(foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if(updatedBlog != null) 
            {
                //show s
                return RedirectToAction("Edit");
            }

            //show error notification

            return RedirectToAction("Edit");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            //Talk to repo to delete this blog post and tags
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost!=null)
            {
                //show success notif
                return RedirectToAction("List");
            }

            //show error notif
            return RedirectToAction("Edit",new { id = editBlogPostRequest.Id});


            //display the response
        }
    }
}
