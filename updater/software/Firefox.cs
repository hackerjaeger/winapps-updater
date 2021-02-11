﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/70.0/SHA512SUMS
            var result = new Dictionary<string, string>
            {
                { "ach", "510587830b6b2b52af8ede23da84d82f9f05d5e4d0c7576712ab08ad1ab80179cd6deaa7bfaafff525980f149eb228005d1d248246f1ce1ead9a993a83fe0f3b" },
                { "af", "32d619eb5c71601855ea43d47a8c694d58a808c7e7fda2c583765bab00863fb0955ebb0b8f8558205cdc9eeca3756266172455a165ae0f9447598e3ae5e89451" },
                { "an", "ecf608f1c1022a28a26481df3e6239f044a66440ffdfc21c3167bc3bd4a6932715b64e87c833401385fae9fe12d17dea2cd15d372ded5139d7aed1a63e34dd32" },
                { "ar", "c2f657ca1d6d71a85aaf4af36a8faf77565faa722b2e735ffdba187a4b2b65de8089b244fa25f5ab185f24fe5251c5d4079f107515025f3e8c1acabacd3ca00f" },
                { "ast", "5db920964f26c9484608129293ddfec85b47838698f5cf08ad6f426473c5cb728b946034e34758602bfb2a7d930c03c2dfb779dd29b411f562e84a65f1c40281" },
                { "az", "b21a21bb974e9058a58658ae806bffd7dd1b370eba4cdbeecb5b22a9f4064b04491dd6bd28728ae225af5b03089c9f0265542e9e3a0b3603eb86d5846964ee51" },
                { "be", "9eae341cba74b4b301984f1aefee5aac91745e650711ad9c6dc0e00cf1ba9d60418dc71df590f89730dfcf3ccf82de983c4baf95ced6d7ff6761bb57cf1c162b" },
                { "bg", "cc5b3bb17e22b1d5fa8263e6e4c9579fc3a75af946a3c937397e2fc5cb15c1ecf09b3bd6eda465b2d24f92094cddec9e4e49fd2a0a87a69e6d7d8b5d2029252c" },
                { "bn", "6ee56ca541a930cbbfada626c36c7992f5092980a9978744d7c9e26bc1bb791f5239babbd4b764ff81786a4dbaa42b8e5cd270dd195c4abe066db7d7af178692" },
                { "br", "5ad6b48869cd8b26cac753f76b98365889cdcde1176fd6395981c7ed61bab75571951d543655aed232eca99320ae2143197f654bccfcb073185b2af80e9ee6c8" },
                { "bs", "505a1b0871c87ac068590761876898ab6ad93da5231a0d3be660ae94866888ae0574f0f01493c9bb295b72726a1f852a7d79945babd18c93e14a056d1830cbe7" },
                { "ca", "0aff5508a90bb876cea5c3a9e20c9762cf47486fc0adb4ddca82bc65671a4624b6c3d7392a7ac036f4cfb49fdf40f978e836c51d38fd01249562474d9c570575" },
                { "cak", "f1f8f3d4d404d542160ef5ee43bd7a80fecf830fd5fd842112384530ec4ae7e43db6b63c6ac1d58cb1fa385a76060bf90d3012b368689f27973c7c27365e1b45" },
                { "cs", "060be6048344c7a829eb38c923f168e9098ff7195e9d28d3c023ddc8d25a3124bb739b4c8dd015c705785cdb128776fdc8ec53de247127e6e6743669023e4382" },
                { "cy", "4de111dc78b514ebad5d776c607664a3e56c55ebdeaf91cd721fd89efd4f7213ad8aefe4f177321b79ed1dea44c3c5ed0adb06cce64f5b280421a8fa071e7a76" },
                { "da", "34becd71c000cf6309a1151e9686dd108c55644ca1fd0d7ccaf30f526a91a290a2f6230f58fd956e507033db98386231cee6dd2cd4b5d44addd6d68d4ceaf0c9" },
                { "de", "b8a24b304c5f5badc7cc31a892c0ad403dd448bdd9740603c1af12a8ac35293a5a5435df57a05cb54ea4e348c05e4cbf314d067d2f2d0faef8e5535ab60d7c8e" },
                { "dsb", "02cdc99e44dd7ffd49b7d4c78d1cacb3ea065fdc91b1e2bddd1d181e70c814ae9d468580cbf322ae987de9e9430fd6de6f390bacfb5b31ae718e32fcd2208113" },
                { "el", "19dcd9d449e9465e478dea02474ac621c2c5e2a1026509fbf8cc8f306fd9d5604b27d7754f58de31ae98f1ebfd30c591ab2599f02dcf2836b7aa616e4bbc5108" },
                { "en-CA", "633fd00e5b644868be884473d98814cd897b5954fd78185cb77e24dacf92fabd9d9392a78c1f53bcf2276440d20131773c8beadb562a57f1d32e4f61fc65519a" },
                { "en-GB", "7124dd25151ab80d4c450455aa985066c5d19db92077f9b1f86e026aad142399afb39414c48c31d14887dc4d3d2ad425a3ca539dfe48c00ae0e954f802dc018f" },
                { "en-US", "09618f2f1596dc9795aaa62acb37c1210c3a0a57f4aee3793076096c459b989c9064c8852f3e543129314a1b68d81520b39e544d70f171e8ce785c39143eab92" },
                { "eo", "7ed1392615007b554fe58685515d60a515d9e73de5745d9ad9b717c25ff3a0b14f8fe80648c2a6bef4fef268387dbb7176cba1f8fdbca7a8700d13ede2182767" },
                { "es-AR", "1beb78bb6de1a99bc3345214682c346a5b12e9cd36e7fdb96914fa4c0764db1b4f668903ec8ccd33cf3eb03505352f878efd1c417472feddab82597a1d56250b" },
                { "es-CL", "d9ba4e2c04039c3d2dcf75d2c248b0ffbd0ed4b6cd4f8a98733605d9ee6ca52025f0af9fc97d6d4740b31d472f5b03beadd06e4676b92e2003fd047dbc321975" },
                { "es-ES", "c8cf47e28b660e6b0d3e21dd44f464a01bebff6819ade79dc85e7915d7c3c4a1bcdf346869cafd0ccb97b44e767605fa1dba549a6a3c9cfb7cc5673b5fc91144" },
                { "es-MX", "bc06c8027d092398a751da49eaca57fcc10d20a9ffe7ff922438318fd6c2b14cb4f6b72dc56131832d2de59f13a427cb9711dafb43ac96419487223de1d17cc8" },
                { "et", "707f554427660ff542cef2e8435ccd41e7240a8ad56577a55ec17c86bc8a521bb43242319cb7f3d92244898fe7629c08972c82f544c493bc64388e467dcc949a" },
                { "eu", "1e8848e1721c2715568bf4c54f1aa9ab201c05c076da959beb727239cedf5b20f990ca4ca42eab3766848c74dee245d250e64b2a2c570bc1bbd76d91436fdd6f" },
                { "fa", "eb0499d00786ac0445d315a570c19fa084d0876bf318129603d03d4f4252e13a3a519e30aabc57abb5177bf267c027df5940cf50d1a888b32a153d60da4ba841" },
                { "ff", "10f0e4c12ac707654684bf7d749a7f6f577097193998409431b6a907a15a64a23af1d5248684655792167238e9292da5005baf3b509c3e51e8e4d5f15a9845ba" },
                { "fi", "a54ae56382d3d1c374e36805bcb7e831db750fab3f5f5ee43d2999ca5f88ad2c64c2dfc5600b1897aa67450622542fe9d6bad5dbc3170583eb37184eca6ffacd" },
                { "fr", "e2d279fe44c08e5b096b26f9f6fc63be839aea9f9a5a9518978e7babf418e91f9615e25b9bd9e5f3571ec0107a4c7bbe4e5bc6336fad1e757d3c5c1af0d846c2" },
                { "fy-NL", "ff323b0b846c7e159786ea2992a211ba20e186d2a0931cd7eb6df90a845ab0bd4434eaa6a048d1c2cd6eead6ab38084e5443f1d9f652243d950dafb103976996" },
                { "ga-IE", "dfe0c05f91fc61df708aeb49d3970a4e06206ab60dfb3fe1ce9b4c69c788a614c960557cacee93340e122d81846a062a935c3d33011787e8cfa92dcb9896b5bf" },
                { "gd", "b5508b465a02baa5d91c26e4660d59ace353c0cb1fc14b9e3130f558895827e9ddc284a69935358f97f2c7d8836bd5cde76350029a5d0485431fb4086afeedf9" },
                { "gl", "cdeb07d9fc13c013f3a3cc0bf120e80719473c428661e926f5eda1795035499d295e6eac29ccb6a0c2b364ceb68c54240abdbd68da400cbef28ebb2c1163f1f2" },
                { "gn", "bd0a39b45467192b6f0332d92208ef23ffdc465b07b4c9a7c11a1ef59d758e41c66f82fb750d2308d6b9757bf94ed693ba915b034230d430038b1516a8ac55a3" },
                { "gu-IN", "4a5d7f80d18ae26a3df2982b05966e0fd69f69fe6a3ff93519704b4386e0fa25dce613c239ca3b7449efe97b3010fecc4b4c2fb3ea9958ce237a2543c357fd62" },
                { "he", "28531606c44d0a95506351fea965aee30e1b7b8ad2592866a7866e17d5130b4787bc556a2897451ba1efa9fa19012538bcfb7a3a2c2053095318d50390a584b4" },
                { "hi-IN", "bf3288dc03c351d37114444afb4ab0dbfba9877875bc7f69a727a00b813ce2f935fa23f68e0863397103a9ff62c40c05f76a3518e672ac451f1573cc0b996f46" },
                { "hr", "5b7a69d77ec04149c6d3f6cffef36819aabe473bdeeb55ab01f9a3c0117c2d89e254d1a69050b6976f6a2f090329bca70e737346efc79dbdb90cfce1ad78c6d6" },
                { "hsb", "40c1ae8a50ffc55c273c56c6fa006dedf8c7bd233efee140dcf117c29cd36619a7a168e494ea8e8ac71a5fe0270c3245f257c4f380224817565ef1afb9f6f4dd" },
                { "hu", "1764f244935d309a347f7ea719bb46b31840ec733c177fa514881c8132ce8bfd043f14cd5cafeff29a9ba9c4cc6b0561abc11ee3f99e4ca658af4a8fac8c062a" },
                { "hy-AM", "1638ed2bc7474aea4506f0b18d769aabd8f82de7c7ace916461d7acf434150e23a9652c4896405c6fbd2ed1dd3aaa71b9cb432353574920f5273662dcff88b83" },
                { "ia", "9409bf8b334e068564aacc5a59db253d6e8e91c0dcd41d64c81119ac24cd0d2d8339148efd04d392c70b8a47e13a245b580cbda3ca64717ec498eecc8b9c1921" },
                { "id", "cb660759baefdde2481bd7c9481e7df002622efb95b2de4ebfafc55b2b214ecf6735e278eef0f6f3d8fe7969dce74a0c6275db92d56c5132696c17702242bace" },
                { "is", "e4bd7d04476a0727b457abb7651d6c4bc46c3a9bbc7f2cf86c257b03af78f246c38e0da2dff6bb7eb30edd00492ef9194b23b38264505bee718c64da69afcd2f" },
                { "it", "de73ad46c06a20c07a490607cdf817d905ce7b8cf96568801b0bf901ac80b7278715dd702c92fde60d688a558728e0a1b2acee86c9e5f2b2bfa65a3a15469668" },
                { "ja", "394ac2907cf26dec4133caf5a9e9c4110cf3dbde0da7cebf9907a843b0acd070622d45de6863ea4b8a23621bdd8f4cd7aeeb407fe3f95b7ccb85d138ef6efa43" },
                { "ka", "2df0d1193580500d32b58c7710efc31b9bce4379c71e15d09768cbc94269926919a69a801815ca969db51f20cd04aa7223ec94b3c944be4bc005dc289955a8be" },
                { "kab", "8d71bee75b1fe339cd09d575a519116129475983da8d4b8dff54b65afbafe8127d58566d24dd073c64af741581eee5475ff024ff6ad8d8db91e61f455b06a957" },
                { "kk", "4361af4547c129ac7ac09ec7487860e6a3ce9606fb45c01bcde8b110b4c087ebe60719839dbd4fa9a8430c6224853b8a904b90c4105a9ea74e4a7ca7f9edf860" },
                { "km", "1a3e62e5cf8caee6276d70af31a9da9d5533125ff63bc2ba04c62a8a3f54edb4bdbfba21357ef43bf0d7483061a3af29b5e6080062c3ad175c43339fd9db9ae5" },
                { "kn", "688ab3ecfd4264f5a43de8c8aa780c12c1dc6cbd2ae6d7e23f2178891989c9b8556309d6339142a2e8b4cc1f4bf6392b2cae244fbc94b7fc59364f9cab2a6a83" },
                { "ko", "01fbffd595129056855caa5d9451afc33bbe317877c4f898d26b3e7842e89c9f81b9e7d5b3ed5627659a07fc7dc3a71cf0cd0e41596fcf42f876c400b7967107" },
                { "lij", "cd03104c7e04dcd3056a4a6f99be30d008b6cae05424f7cdcaadee7e05820bd19252c02b2e85fd33a76954959ae07c3366c6e9452ea812c07801c5fbd61d60a8" },
                { "lt", "52af1e0dcb9f5562552e47880709bc5c46eb3a1c5dd84bbb9fb60f28ebb439b8a19bf1514e2b42bc24244920ee94501d13ce5e6ec4c3745af003ff8c5f49a0c9" },
                { "lv", "73e3cf1688171cddd3a34d5e2d229c55e57113a2e70278dc278e479e9222e721803fb184c4839ccc8ab160ba5c5522ed6feed19ea3d7fd680674b4ccbde2797e" },
                { "mk", "6ae6b7b6cbcba3678d424817c4e3fd4d5d6689c5fc1dcae96f2ac15ef1372c0300802721273c85cd6200f62bda6d3ccd7effe7793021d009c2c3daee5b64c342" },
                { "mr", "2c9327c5eac91e652236145810cb6923c61700af0bf1dcd24c37a936e9c59b68fe32794bb65ee0c6bdb40ed8f98d3639f0a23189d577988d98c97699c5ed60ff" },
                { "ms", "cfc65e906bc6d26610c05f304af99848dbd02c390555bfa73e02741e5ec43a628457560444fc4fcdf385fa36eb337ab395d49b3e12efbfef1e62afbf5360b528" },
                { "my", "07c4e65353251bfc5b662c0ddd9d16f4d7d989da3e74f0b92a3b714fbf3e52c77bb8f982c92914d86af5e6cc81175453ed440d55cbd4dab4be3d83a881bdadfe" },
                { "nb-NO", "18a57c580e4bb908e1b780ed6fdcf6e4e2c9511ab0529efc2cfa7ceceebb031e6eb704ed8b2e86d5a3ecf98180d07bcf5c22b4f7ca6f27946c2133a97a8310ff" },
                { "ne-NP", "762f0fcd250ef97a0923eaeb07e26e9e1bd56ed27634f00b56ea0e35d537c65a88c61e6c60d4850c4f6ad6eb7b1fd61ff69aa9d77b0c0a9aec4775667dfdbf6f" },
                { "nl", "d341a90f0fd9cef2c7a6765b1ff2b082c8d57a7f3c89073aef5a43675163d60e42c0929d97554c92dbbe4812e554d5ef37eaedd9f03645762b15a7ca1716a03a" },
                { "nn-NO", "c34a2675c8782b76ce661e9579b0833e21bc7e698269a346d416f51a946f94fd3aa9b7cc5a2251a3c10bb88e977d6e1ba22bce56e3e44a9a684c79502a045a14" },
                { "oc", "26507e34fd0dc3c0828ece90f91566b3379737fe449d7ecd89211f0fe39fed50afd694c605f690130ea03e90d2f2d5e6a27e4dc9156f226c7f0df1293afc3bf0" },
                { "pa-IN", "6ef6d7228d12ec6e2f1a3901c99ad915397f94363de79c0e8b0f9c01bed438a31dd8f2bec98f01b32fce9aefc82f736daaafdb948dcb34663d7fbdaad96897ab" },
                { "pl", "0fcd38366e3701cbcb2d04b5babd6f6509c3ebad094469bd20d6a1a519a738e7b40e09750fd19db3ac3dbf6a257b0543d75e2ec467f7b50b04c8d20c5f58400b" },
                { "pt-BR", "2c0780531f1ac04424e0b3c533bba69437ee1836dfecb0874f27f673643729009deb089dfd85219adc0c8f0e9362118c54f0f340cbd7ac7290cfa7bd8ffe2202" },
                { "pt-PT", "50627dd5640a134fdb6e23da460e973dce95236942f78141047ab2835dd3e051d2c09d7e4de707ce6fa1a2d52bc327cb67ac6ee3548c5adb1eb2bada353d6b7c" },
                { "rm", "2d4a7e95bdb127af9469da8bb45165110bbf9ab4656dac64149c8dae1c0d501066d03aa75986b052892ed6393b8f98f9958096678382543693240481e8ce3012" },
                { "ro", "728c3ef7b3aa5baee83e40cf2e9ee23ea5d8a591c78a7a86d5688b006994eaae5afbf2943bf5bef02ef307b033e1007b3934119644c89418785444e1282b9a95" },
                { "ru", "ee93ef8106c2054b964729c49d393cdd13af897d0857c320379f216371ee635885a204f9d11e446dc63bba6564b10e810025942256cb92947d4429e939503aee" },
                { "si", "c0f443ce928170c97e2be9fde8084ecbd0e4c3db872599f3173f1a1bce823d6824ba0ff66f929b26266d4a0fc952bf3fb7501ffcddbeee4aa4fa7aab8a56a712" },
                { "sk", "99b6a1daf1ef691a4932235932593bef347471a6b0a0bd21e4a095a414093bc06d2c1d9299a895502c909c78990f291a91e5b6574b37f803a94f18b82cb2b249" },
                { "sl", "be5c76bd23f49ec693ae301768e2b13a8621b63f8a6e722b6fd5fed6bd73131a626ddb3826817590d5a7a0a9d57bec68b05d08a801043e5d9cb21d3682cb4582" },
                { "son", "f4092443ca5a721c05e976af27fdfb24f4e595c3e1306e8826bfd8a5d5a4ebb23d425c8e8665d6eabc87127c508b0e8777236594ffe6c0b256db22f428f1d627" },
                { "sq", "f89a3b3db954b6cabe760dee8f98e31aaf0038c84f2b4a7ecc9fdaaae23dff8c713015943df00557f8fe20e9b9010ad7b555b8804f8705c37a4ca45c6bc6cce1" },
                { "sr", "cc1ce1f4841ce8b4390b06fc67a98e5c54ba50a76c8292805bde750848d76a36e354889e95479ed4ac26cd788c808fc324013df1c3f0c7af901702d85fc0fe2c" },
                { "sv-SE", "b2b4f2f6d7ee35676a925838f61ad44d082b7e4ffd12eddb5c24590e9bdae77db28cf553c4a0c9e6e33436d75c5e2bf11c98e537edadc9b9cfcfd5b1cf6208a0" },
                { "ta", "0f97a9da0d5119cae28a0e00cfe12518fa9c220c9380cd8b570f5dc8df9a1e92f90194fc7884934e3631be9cbf0f7acdc194b518e4adf72e7b288afc0dd30db1" },
                { "te", "88d25233037028011c4352c37a4807810a1da8b5a46f6663f51634b552bc21d261586eaecf6896cbf6a507ef01a9bd8f8704799bb1a8897c338c048d5e8ef6b1" },
                { "th", "b21c69efec2f1cddf769307b347380520f9d1fcc00bff661f544b1c6d638d26f8ae602c8f2efb1a39512efb1fc7d4dcf7df3c1023d141a1ab0fcb2eb1ca44247" },
                { "tr", "08920621da1c307e6167d34ba477b52aec9adb2076c32672c465e96218221337e4615814676bce555930a424558a345c46b7b59ccb3b4baa21629a888ac1783e" },
                { "uk", "afa866cab176db4aede25da3174754c20cb7c6aea68d020ef65103a69d2eb672ea03d524e18dcb9942001400c67aa09ee9138bcf0ce00c2b58dd31dc4bb4b84d" },
                { "ur", "ce85dc47278a9d43544bfee7db5a8d4c15f604ac2b3fe6979198405e84124777e7387ef33437101172304a15ca4b1c165fab663ed7b436d4f310dad0f6fda911" },
                { "uz", "07de89b1ba7bdddb2095f88148cca9aab58cfe16c99cd99c6160df8cc1d4be4bcca84ce0b3ebc3fa9a5e8856127139a62872da3670c1155995537bff8060b76f" },
                { "vi", "fb924ee0e65749de7cec431be19aa790aea724d96b8be37b6a39200489fe102954efc48208ab87acb1bea4919d65b1aa5ebe97fefe7051cdb43bd61cb38a1df1" },
                { "xh", "047d1bb928501fc3e9e244b8a05dea601643a4a4d7f95c8d3e40e22216d4d17071827e0c1929fff820c60183febdc7264ec1489b23cdf6d66c38f7dafe063e27" },
                { "zh-CN", "2506af9bea070daf8d8ca5cf389d3f09ecc218472447bbc1aa4dfc006665c17e81648d20805291a085202e43d0c23a9762ae69e43f80f8cd4ada53ec569668ec" },
                { "zh-TW", "86816851a28a6daf9ed68b87c421013775f5a9975e3263e3f17dc0bfbbf51cada29d661f4c4b628775d2befd6ee152cc2ff0dfc9d0a911077d0f368bcb5f30b8" }
            };

            return result;
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/70.0/SHA512SUMS
            var result = new Dictionary<string, string>
            {
                { "ach", "4008976b30e6e147ecf26119f0fa8649919bb7ed4331d753122dbed3abcdf70260c18d81bcb8f0a3735e6e225080f9f7b9f2a4727563c92bf7d6a0d09a6cc9f0" },
                { "af", "28e4b5f3d3529d511f068cb4db845b0e8b420858844937eeac37eaee6a33fd60e265113bf7dd200325d08a6e0ed4ab0ee1bca0004cb944f48acb3550482b3362" },
                { "an", "70ce128851fab2646fad00fbc12a52cc944ae7cdd0768263b257e1a55dab2ad92443d08a437df86cb6934cf63d3aa4a73449b18aa3b7687aa7bfb9d7f3109101" },
                { "ar", "f747b1e3203d26f8b0beb40ae7c281c65a1eb45977f7645e316fbd265ae27bbf37902a98dccb44a2feeddf74da2b8cb7914bbce464cdb106b974e00782a96482" },
                { "ast", "99a28bcf5f74ed6ecf56d2de574196ab1ebd89975f8341e4bbc91499e1be3710c87defc130403638d3594dbd9ee62fe54e4ef4e5448d420499470b3ccedc70b2" },
                { "az", "c1c3c54509f0730710ad519130b676278e729e249344cec7c9936c569ca1a6727dbdcf1dca77566cab16f784a3b64a15ff68b7ffeecf3f5faba367d8b7b3c04e" },
                { "be", "159baa6c796d78c28b85c86e213abf1f3027c00da33a786773938c1dab2a248a636fa535b4987f8ed1144358afed2b652ef458cf915a0e9f07e3fd0447c4693a" },
                { "bg", "61be4f87b9718c77eea889666c36461ea0c13d108f79281b2554352304dbabafb6ce0c579e0c8ae30a4a22613af742830b21e874c166db05d997461acf6a5599" },
                { "bn", "d6b1f10f91b9c387a9b807b644e3b826bf5a9756432ff1d7f7f25bf46c6f7dcb5ce80ceb307c5ca4bdba708b1fa051a1e365ca4a4ca784c14499fc3fcb1dd640" },
                { "br", "8fd429696fb74f98e5011915428b997d2e4d29ea55f46b36b6b2dc6adfeb4d92ae55e8e4c44d3cd3b7d8322c23c603798cc3eb48024a96f09efb2a4941dc271a" },
                { "bs", "bb543f252610d3a0bc703d5b565a2098255fd7d3eb31781f68d442278a9eecc565ace1df03f77382fcd9b7cb021c27c6e0187aefdd2d4999a5f436a3b22696a0" },
                { "ca", "4266485165a79b7d59a6b5f59c36b96fff024fa835452df4d1da596436d9905eee96bc2a8959c0a809b495e8b864703faa9a1d8f3192f4fe4b31663b52e438fc" },
                { "cak", "0a685afb35bc5b1586b52b8fd6e7a1a81d2c6ef176a120d2ff8c6c5852964d7e8d047ffcd31b3550aaab52bdaa9cfec04f5c90553c91383f8c9cb97473843734" },
                { "cs", "7adcad8a7ca9f8138b6eae593f99b381662aaa117300812933f033b1f350ad79044f3226cb6668d0f5b0edf25d27449dceac9f8b7f1bed9ec98e081a6b96bd94" },
                { "cy", "bec4f267cc63c2101b5a333876fba632284cff3d772768f3f62f16d8ef8f0fcb7b8dfe089c6ff55538e00319429985c9a7551c411081fef7e13329f17f90f36e" },
                { "da", "4f0d65a2d896199e1fcb200234865484350e1010f7a06ac95faf81abe46bfca215cd68aa523bae9093b6795e0944d0b1a793b570b0ddafe02b44dc52aa24a6e3" },
                { "de", "743f101a3079e3c4a2f80d6a1e47bbb0be43e4ff8a56dd41289987fa4443e7160ae67781d9149e310cdd9ec5b81d1e51c048851005945bf019f63618ea5113fc" },
                { "dsb", "082338c61b4c06ec2577e7786210fa334cb6b2f8d309b90a2778c548b5045fb69e742ca38470ea96f3e9538137a160da3fe510bcd55e48e165217586c1aab89e" },
                { "el", "1e79b037990b7dda9c7a838a1d6bb9d6a5c190b6efa5c673e24de48048dc88dbc57fb66a0231077b3abff4bde428d21dd951df63cac41a70db28ca8afe250b45" },
                { "en-CA", "c93c21fc2270836b34498e74aab166883d82d9b422ebcbbbbb8ba2279eac5e83de9309557dbe672d6427c4219db14f292c85ff733b4bf622dc0c1dc15a89a601" },
                { "en-GB", "fcf4a8fa9d2f5b6a41fac34504c96af8ed1d70199772ade29a8d7cbb631f8f5702388f44bb01eef87e7a50587c2ff9e22d982a2dc0aa7a3b99c538a0ad9f7528" },
                { "en-US", "926889109a3fb99c65d21574c0dbba855155d2ed7afd4de21e226c1777eaa31c546c6a12159c2084d48e5f18f66e89747f652ad2eb151dd2aed0f9dd16f061ac" },
                { "eo", "9e72c2924bf6faa2bf43501dad9e9d627ee798b422367680ebdc41bd3d71bad2a6eacd0bbdc4bc6ed1db4d9d65be3cb98cc5fc96ae872c4a819a172ed115a076" },
                { "es-AR", "bc9f065490cb68b90f9d845fc757ede8e6fe2ff603f2dcc4356dd32b9da1b7fc168cffe08b89d82dcdc07905c497d3fe1fb124aa5e26d29b463f16443b87e29b" },
                { "es-CL", "ff043f0700334b99aa04eb36aefd133b230a6a5219ea8e23f78e1a707b65de4380ce6971909e7088845da378e7fa562854dbdc5c0e488ba7c2e4bb5c55052d67" },
                { "es-ES", "99c45d83e270e71fbe9f6282556cb4ef55c9c77f8dabc0d66479ecb3d6c27059f0893835683500b591393fdd6d0280ec1f78377f04ac71b6f64257d693467b4e" },
                { "es-MX", "35b59a8bc393b954b11914758a67a95ab4d5ae4d94ca15e547b663e26bfd60c6b11565274a95c2b8b64ede215b078f13542c4e7573b1600f8ac084afbbec8c87" },
                { "et", "1cabca834dcd3d2c1d013a8e3fe96734022c0a66027dc65e37f780916e28a98f82499c734d43afb5f093e0b931502ee7935177cff18401a333b1151723385cb6" },
                { "eu", "0c3ab0165808d4d8e40b5780b4b2c830e7b920005285c02c0004254ea750f3b122250872c0af7f9b034e8f7cade08430dacdf795cf3c871f940f9cd18a392615" },
                { "fa", "1395ffa2ebfe3022e2851dec0abefee68e2a27fe02364f4e045a74d47ec3d0eaefb925e7e9ea2b202964a04850a59497059ce4a766a3cf597098ce4efef1fec1" },
                { "ff", "c2b1aa4eb7c7e175df406f106d162a24cd535b710e0f0fe3283fc25318be068691a1fe18a3ac84a55567f44b55d5e29a0848ab2df86dd15769b845d25b56b866" },
                { "fi", "c77d3d3999739aad6205647ad3d1eafafc0bf88a759d9d7a559d04bfc71b9722075ce44efea2bc3484e90053fbaac0a582b4fcb6c47a26da96714ecdbadf2257" },
                { "fr", "d257f24375c659d0db687365c23b953bc066cd4de0b39a9395ccef70dd4c4ae24f8ebbdb02ba01cd6b3c75bfad7e3742504608d6d190b5b00ee86de2b275d6e4" },
                { "fy-NL", "1465723d0fa35d9d52699bbd9e219d462f270508f46e7874e829742361bb51c91b14716578a2227078b53d8f3e1ce22e64e9b0b21ed9d31dbca51df8867df170" },
                { "ga-IE", "9a3bbe3f34c3960711c302abe862d2875b7cfd3598573517f3061c2555db1975d348476a6050ddd40ad7d1bee9a023dc8c4796496b6eb18b1b4d23242acb83d8" },
                { "gd", "b7262caf3603d53e43bed91dad6df4fb07d59e514961c4505a9115f60095a7fa74717706e51640c07e8fbbe11463d011d7cc6c0e1a528c43f45edbceaa2d10ad" },
                { "gl", "3fa157603420dcf53b23abfac2724e0ef33e540b4e294105ded11c7ff5e830085ceaab4b40ef1fd188f018f4b7d506b5abb2908cf71e791c665456224cfa93c7" },
                { "gn", "df3ffb65641ccca7b741f942a150fffedb6255c8e8b874ca30eae65dc6fc324c64b53561adc43a263f9b54fa53b447ae4ee07bd62d4a3cda55ee07c921d363df" },
                { "gu-IN", "67206a20dff3ee7cf8f1d2c3393110eba626f860f9c4ead9770ee0097b9cf9b51be081296dd9a5f5793a05d6c460f61729ed7859f8223985bf4f98b53c2e7564" },
                { "he", "4bdc60d1621542f47813aad190c9c4c23b216c70aabf9d24fbaed37f04cb259e3fbff62c334eb860356268e34cf945a19def1495922304dcaee90abb0707a90c" },
                { "hi-IN", "feb71e8ca49281419af769bd0eb7107548109d9e4dfb695567890e961f776dbeee3abd2ab0e494a5b5c56883ace9a2ae41104ff394eb53186f4d703fce873da0" },
                { "hr", "7c7f16517d20efef0effa54ca70de7c2cf30ccae57be38377d21cf5311c45da8fa5d0a3cb761d07f09ba59030bc0daf97557008bda36a0004ef8bf59d149d17d" },
                { "hsb", "18a01e5bee6ea7fe8199d24adc4fe1b665f92d35893889d8aea43e504f631ca77313fb3a60a5e9098392c7b5ea17404c53df9879030e035c57cbf267b659e741" },
                { "hu", "81357494d47070d66fd72e8748c4e437ae7528870bfed0e7f8e406d76bb6a402ae5005cd2bc6f0d9be9dc5903ac170c599039462c12027aa48074b1191c616f2" },
                { "hy-AM", "8196cb9026bef65a4974094cfe3986ea4696ec2fbeef3a4561dd1affce861f7ff8580b1fb329c4d84f023c09798fa61927822989e0d49134408c0ad3da9eb02a" },
                { "ia", "97fe9a1743f523e88494842781d480a79fdd3327ceaf960cbfdc1b38be44ef619a39a13ccffc813a01ccfe3b99c63596a826466fba88452ed25bb8316fb0e5b2" },
                { "id", "9ce10fdb9c2c37da01e03eabcc2ada5eae5957a14e653306fc733ac1a3bb7024433a04478980823c6dc881acbf48960c7ce6ed7fab3ffe6c8655a7cebb30a7ce" },
                { "is", "7dc5312ac0768b4af8c9509e945a7d74e4136aa58830b58a4aedbe57c0fe49673e0c1929afae7dc5ab99c7b1f379ae6127013a76337df2fedfefd2ceffacef0f" },
                { "it", "03f115fd1fb7b334047e0d4831b2711d35c66bfa3d7a91c2f8b386f862fdc7321b7fd4274dcc8285d3be59e2b9e43febf613f6c50c958e9366800ed4f9173bc9" },
                { "ja", "7643ec3edcb1d5081ead68f6055915188a6f93a42a0acb565aa77a2634d932d24a1808251fdde68d7992c8d6054d9648aab593b72a59d34c1d0ad378fdc4607f" },
                { "ka", "2072496021680a3e17171fdfa29f6e08e4799a23da7b3036cb5f6f8b678d5d3ff57bd1967f3fffc59add35892c822797b6effaeb2e6ef52befe1c87969a9b1b2" },
                { "kab", "1caff0752efa7f5582ff038a8669bd70ee139d2fa3b82d5ad53b3f5e6f4aeb8fbb5cfbca8b3023eac54ea9a410c3d7d6976c3afd19a03d20d58e6f89ef4b7132" },
                { "kk", "c85e041d506431095646987ad5a87f4d5681caa72ea37d85a05f9d14b7d63159d6c8793979832ba8913f543e393336a08524103b09d24a55ce5a9262f2883d69" },
                { "km", "ddbdefb201c251845dba08f7515f7d787e29146ec36dd2d203d0e0ceaecf9d5fc3298e19a571dac3278225cf8810209363ff676b08ec0084e04d520e4a7408c4" },
                { "kn", "e1b9c5b88e018bdb5aada5b3f050900fb530e180eda04e7ec0f31443dd301c086381b5ee933c7ee1ccbbf205d201f402f9a594f493a7fe9804b8c3ca278f0fdb" },
                { "ko", "f333fe446a632386cf1f7e7a14f451562fd914f5e5be1fb4537b3426bb819b31ce94a38d19137231e3dd671be1a1b3c378038f8c38778215caaf32e60e27057a" },
                { "lij", "41495d9e62b361e3e15e09e42a1e3b3fc6801b5ce010b2b4c9309af048410ac41ea76d491a685eb666a7d9993a18b160a5e18adb38ff9801ee4b0d131f913205" },
                { "lt", "d6443e4bbd86d22816527989cd6f254193fea2806894a94bcb4f070662c127340e28639c80fffb4f982eae39597167264a8d879da879674880d1473f7f6cd32b" },
                { "lv", "48fb65e6bf336a1a9c5fd7d850a77b5a232e6fb233c84a170ca19f6370f907242e79b00581efe8885bac5b5fddd92a8a8267f958a1a1620e82c363449b7ca3da" },
                { "mk", "a96062f657d8cdb7446b056632e6ea949fc161af8eb481fdd30d1bb53981adb9c6ff5ee1d51e51fb594523e64ce529e160f6b49af8ea600c2d3f821cdab1935a" },
                { "mr", "5f85ce371b40854266b9c10757ef1ad1fc5bd8d5a9e411639bc0418ad2b16dc3c52bf446ca256ba04723cf76fd56af9e35ed9de5eb34958ca24aa3154d76a8e6" },
                { "ms", "cee7fefe3fb3759dd74156cc9105064ba68ca9a23de236d48da6078ccdcf99265ec148cf3e68eb4d5c4bef685740f14ccb582c09ab2f7615af2b1b351f2d58a9" },
                { "my", "0ef0ceb523a0735ef806df66e581caa99daeb98ce096e1ca75bf01f68b467c0e4b64a565f9340fad6ef74ddddf2e3fd8f60184074e025adc0cc63066d6a1813a" },
                { "nb-NO", "a613db1badba2ccc7cbf389daae73608a79e52f11018bf32faa0f9a41f5f852c8d0964818e139132d91209030d8490f07dd06028967b5ff4fa5c6111765d8c74" },
                { "ne-NP", "1542645191d7ec9e09962307d8ffc906b8f9b172e5eea1b5ad91a9f525e9de663cbfa3c1e00b7bce22450450b2c2c91fe1c84e77fe28a6aa80e71bf446e05995" },
                { "nl", "34eda05437523308885e836e108ab06247d8da5421f7d893be14a869f644bc4087f6de5d3e1166107a54e545bef2056c9eabe67c4ff234b0a0c4429c82815e6f" },
                { "nn-NO", "6c089dac158ab93908e9a5a29db95b5397a8637406ec347249f79b76ecb1d4ae455fb3bfb9909200e92e40419eef16c9812ae66aefde419a960593a58a3a0d2d" },
                { "oc", "b2266a343f5588de55cf22d2856215f789221347f4d1c2812de78d3c38c0e3a5d922e734f39b93d06d7bc1ea4abf741b7f838c8ac2b3f984d363f943f5972d5a" },
                { "pa-IN", "ee730e0890103d33fd6eac43e28e91af3e79f58c4a27ee2950391b5e61cb36fae80222143ac0ef0b0f06d8ccca737cc03338b405fbe71d9a2c33272b5eb1866b" },
                { "pl", "005c15a48736e1c337bb73e6f13ae7d40f57d0b912ca3a36d3b0863183d415368f94fe061d8928f538163eae593e9f356a6d81f977f82dc2c5795c78b9f86a67" },
                { "pt-BR", "348cc3366d2c1f3352c4fd9d3685412c5c58295571dc31c43ec9952460dc680457c87a780d3f4b343dac614c55390b0068ef478d783b5771d898de471f8b754f" },
                { "pt-PT", "0fd90ae6ff6269f6da85cd878b0829cc3f851a2c17ea8c82889e2cbf66ef108e447f85f9cb38abea2229c30a18473cb3b864f1d4a9b76ba120fcafbafea71b8a" },
                { "rm", "a32d639424e001783f00fc557974f9d7ece2f1ecdaa1c169d998203866d6967a99532fc25da1c40cf2d07be05305fce5eee2da763528e500e99436f74db279ae" },
                { "ro", "6108a54e6248c81f593287018c51eb3c0a49d188b65d2300bcd9ac6bf7cff0420e19f27737f8633fe100f5b1f7d2340b3a8067abda840d2ba7c77950b13c6544" },
                { "ru", "e9ac1a107fdda6625aff858d48c365a6336557363a739fb1283f44f033c9c253667f42fe67fddeb4d27da0fbe817a2a15f09c5ef2c3a020e82ca02988542fdcf" },
                { "si", "9d7bec815701d8dfeb5ef44f9bc6e2d8151f807cf0be76f38d6651784ca94c6abde57e799503827097429226e985a85ca1ae43c6e84464ae4b0828c9023ce243" },
                { "sk", "4d271ba6e647612f06ec40f2332869b4917d2898d9b1bbf1929cda7cf172645d280c95b9e60b8e8fc95cf6e638831268e5a40a5a795fd1a5abb7410e946ef406" },
                { "sl", "870bbcc541c1a887e30ccf2af2846526c0799c203064ac7c264eccc4f2ba0b6a51160f51dc0192dafcaddf960f3b5d2550ce790b4f64719dfe8d9574909e4d34" },
                { "son", "4596ee18bf518ec5fcad4e6c276472d0091af6e8afe6fee1cefd70a423a0201a377486791406152dfa267672c1fa8aa1d9af5c2dac453e2189a8559af57ce900" },
                { "sq", "ad54141e5ab83a061609d937d7018c67a2f552ffc4200010eaa1c5f18e1113c3578788c8068a6f7859f31c6124316686d38a77231b3de4dfa054d02014b42f66" },
                { "sr", "20e969395d474bd0b14ed499a5d3d7038f0f9c65c928b9fb334977795f7510709a0a589559f670b3676966b8d546fb0d06d25a407f77ccc2b28decf85adece67" },
                { "sv-SE", "29fd60699e7e5e4fb2362fb7a99dbb75136d217074154ae909c9399d1aa1af34003a4f45a2701c3908ad52f0f329ef0515f6faf181adecff6355595ddc964e50" },
                { "ta", "912b593b12ce531f3caffe33ec74d52e3ac824ffe6f75631445c6b953a3838ce170bfc9fbc20bd24c6edbe4ee98cb26fb842a2056c5e85880610b3c2d26e244c" },
                { "te", "4838b0e1a5b6d4f3bcd61431618a821f81f6fb5f7f36519556cf446a264aea3e9625d5295a0384c1802ed2eaac71345cdd373f21dc2f0b5bec10338bb0552044" },
                { "th", "e19c67ebab8a3c6ffb73ca4854c68b2fc08f8a9568d5c7d711f89fa56d701ca2cc5dcc12083bbaf42adbfe7f04615b8362f5cd8d086854a4041acdaed6d8c2ae" },
                { "tr", "22ef2da9859ae061aa0597fd58b1d363de5aedc721ff5dba2fdc5e91dc3b5d613ffc8305ce7ef68df16b417356f50bbd62edf35f945d94d94631e2619f3aa22b" },
                { "uk", "d100394bef3fd7dac620bc04ce92dd571c3f57d2d91e386d3098e0eb1b89514100224c19246a72d25fe0bee07f4bc5f182b708d1f605cc74b56996a029562b3a" },
                { "ur", "1f4e1887a183aecd77920d112a91d39674d3ebdb9dcbebda8125ba2de5e16ac5dd6d6839863f0752a508ee07419c629b161d4a846cbd002771f9433d387c875d" },
                { "uz", "b25f82810096be603688979bd157b3f1757ddd973faa63348ff6c250854e610416cf731b0556c8e086b4932890a8412575ebc4c7de08ebf3519730edb5de79a6" },
                { "vi", "b50e74ccf8c8e8cd462b7fb81921a61afb154c6fc66871d8322c6b104a33e098a022f8c586411a4f3d7fe00f7d089d9f1ade1636ccb31939768a4127606e96a2" },
                { "xh", "ce79a07c36183027fcf8e2a206f64eb28aeca5e38c9984905c822b215f9b2dee0883c598d600ca5fb4b376300dfba911f84ba846c332e523e674827344eec395" },
                { "zh-CN", "6cb6cea0481f722d2a8e6532336280cf85c5152191af4151b75a71ab29ed0dc6eb8bfef47b7e26b8ed3ae82450ed65bf23abedbbe8a0cd4125f8151a648d9100" },
                { "zh-TW", "5bc0e86b3a081450d45ff8cb3549f70ba871741310fea24116571d07e0485e3cc400a40cc4be9a05334f31062356954b0d6509275a9b6be1bcdd53e623f38a40" }
            };

            return result;
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "70.0";
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
