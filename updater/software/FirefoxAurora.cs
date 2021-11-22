﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "95.0b10";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/95.0b10/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "9fe0aa2a4d8e7fc78e4f80a8ca6a5272c42b747411154f34fbc0ae1a1f5337164e49fb1f602e4e29b60943e5f0a29edb918f9c99d6795c9c9c31449a0098352d" },
                { "af", "ee845debac3bf1f52885f264d5deb3959016b509dad57763beab39898e12de3ec7e1807f3f784e833f55493cbec2b6b1b51b9bf5566b2f68928daeb9f34b8e92" },
                { "an", "249d3c1e3337605c8a057a8e22e0095d6ae77852e593b2211ccefed43e1dd73c592963d9f38e73b681d9e78c1bd904ee3bc47b15e5b194fc07326a56e492fa56" },
                { "ar", "f804edf318a3d915d94a2aba592e666c6143b55e10818daac26788ab3dc2303c03d57f14a2c87630a15a87a3f9abb12e9a6f3d19928b1ffcea70da45605cddcb" },
                { "ast", "bec3fba34fac58b5f73636ea03a5a3b68b357d05c7d56cc14d3f046c065d354fd07cf526edef491c7b2ef8a65c6ccc0a7d5266234324d9d76e9c9c6a699647f3" },
                { "az", "2aab5655a77a89f608ce0ed827fe27ebe890cbc65bdb13a76aafe910e0872f80f6da8b044b3cdebf859abc2660fdaf6d2e7fee42ce89319948bb941f285c0084" },
                { "be", "b685a4b8eea8b996b4834c918edd9f437d4baae0e42a97370be7f26f928ceeeac01c767f799144554df6587101ec25c3d432d5c39a1d81b416b123d8a2d12113" },
                { "bg", "aef456910c1856a20e74440bf084af52e32a768d0d95590e5dcbbbdc7f259b2f5bc4aa6b7366fdbe3d5b0ef4d05adc88e89f062f19501bbc02142cd8fcdcba24" },
                { "bn", "9bf14c7749d475b0443b1e10ded0b164539458053f8fbb1e3ca6a06e83a3a450657677925535167c175fe4d692949b7d38acd458788c38eb88ae82755aa4425f" },
                { "br", "309e8ebece0b8d0dcfbf77717608e76d82cf9ff97a9ccdbec00ba49e45af28c4a94b866d2ddfc78fc984703fbc8d13bdb205e1485ad0b3d9022e44919dea44c9" },
                { "bs", "a6d88b58901340146299d90bcaeb1c8cf37b1153b3a4d65a6602932a8776459ce222966e9720baa1af8c1b6eb85a91c7922f0a59cd51932dcc5d596a7d55db51" },
                { "ca", "5353989cb8b503cea8037fe1414fffb4c704196a1c7b3a3d893ad05ea93535409c4b43481e4bb9fcc7b4cbd843bd843110d8adeb702aa0f93ee98166b9b9ab1b" },
                { "cak", "4d0f7a82882d5c49d20e7425cb518b49a8fb7fe74754b2ec20fd05d1021de4e85631b7d27c7b694ec036e40d6aab938f8e6b80c86885af44eea3c67144de22f7" },
                { "cs", "ca71b36c9ace6da620d1a0bf53df5de1186266d771674b7b65e109ec5ea5a6a5664b6cd650d263f35115073d484a60067dceb0f2092748e63a9a4ea249d0d31a" },
                { "cy", "d3a98b5afd939d57f1c9c226aa5bb8a3c6482a390fac634f6b4e3e02bce69ce74b9f70e1522ab0853d3a05d9a4edacf84392f3e01ac687637df5e2dce03bf4b2" },
                { "da", "0b7be2f4d44cc033307a9cc210627906741ee6dda94748912743a05cd5b12ad88c76c676bb62f0584b1fa81d6ecee66a3c3ac99bee247fe3008db95f14b625bb" },
                { "de", "4b9db4de08300c235436cd4ce24fc6c5eaca2dfec58ed208ac52d9cf50ef7656dfcb415f521cacd9f3b5cd02b9236b66ae6e92f880a21b1f1092d717242423e5" },
                { "dsb", "7146bab267f3b98e880b2b7233c9065aa1711a7aae5a86a1a51a2cec3d18a4b5b6d779a00c47ac50232fa7a25b7bee7676bbcd90d16a6beb4c469044c9ed400c" },
                { "el", "902d1656d046b71bf97ebf8dfe17209836d9127b2c67b58773dd051f351bfad33d04fec5a60a9988afd160933cade2ce396cc1e73a73e94bf87f74c006cd5d2b" },
                { "en-CA", "0db209e5169dea8af1dbd5a3671af82be429bb5b1623d6982312a5f6541dadeb83bd9d6f7da511711ceb35da698b64936acda838eecf5779d6c83ba646aa0384" },
                { "en-GB", "45fd34bcd253e0382071692c39f35065571f9f009eefc861e95c092d2e0e4b25bbe87c2c7cc2889d1f55567056cedeb90e2552c9447ae7cfa1a5ed9a8b170a8f" },
                { "en-US", "6009a6440bae6742f2d7909abd79b9e1af6692ddaea5cff774059f5d7382df32b64cfbd049934ff483bc48c24c69d0e6d4469b15a4f763e982536dc900a67001" },
                { "eo", "aef3b8e58ff1f10303d5946bce0bf0bc94286b2e0e7a52b9954f2644fd0af99ec85c784626c54c0fe7b8509f7ea3c8bc359d44a46f2d0817d000caa334f53c97" },
                { "es-AR", "99e2265069c58927276fa15ed53e2084b3df6e4fb6e1ab07c690f81da9ec4703f8f5deaeea21a87a23a564da08be19b2cc40813950bd5a4dbe8b468c0bc9e1a4" },
                { "es-CL", "18ff973aa6f325b48eb96c0bd9c9dda07aa0957673bc2eeccb98e4a1ecc7a72aeab2b7f1ed0642aab5ab8f940666719eefefd6c8417248a78056ff801a30f55d" },
                { "es-ES", "01927c68527dfe684efee3a7bf0d6e794ab3dc6f8f836cfb35b33f15ffe30b603564ac6de3c53fefeb11270096e758d58ebd60d721abde39dab079a366e16e25" },
                { "es-MX", "9070e9c1620bc4750b9ccb25c2570a4ea6c31695c5428eeeb3fcd60e3cc18f34c62f06efb9ab096346afc2adf55da5526d6eca50b6b201b7f6d700c4b145f222" },
                { "et", "6a1d27d5ae8da223d2623af2278954f5655cf7194f95ad4e39feb2770d5ed3624aaeea49c1cad32966b53d5357f1da346aef43f5666990038cec487fc9139dbe" },
                { "eu", "21298ccfc61664c4c90381dd0f3857664d628510437115b8d3929fe12cddfed1e1c7070a1ec05e6079f8b0c7c74be37d978f8f60d7cb1d7c1b2a7a4f3d23c9e4" },
                { "fa", "32dac0ae8ac0c9fb4be68518179184b279080d39f30b4ef5446a7e733e516e2b6f5dcbec67cad3b95e2bac34599cc8512b867364be2a766241809d9a4de39ff6" },
                { "ff", "0de713b80edef6980e182ddb1c04c9135e3d5d3b5c2f29dba98fa1a289a56b4f2638d85806c39048dfe6ddec833d3a9e8f3fb90d59e2d5073ee4f7a642810c3d" },
                { "fi", "6ef4f96be22151ec4948838bc4ef93a38bde73f39768ea80e818fd667283526769a93bd44c1fe1a87f3e7e9ca6126d940f0760442d5a68479b095d3122c0f725" },
                { "fr", "7c318ba18be15f70349ba74f8a121d0fd2ad35bcb695afdb50cfbc7e9b838202886b656b59136f77bf5fabac4fc496fdb59a348a14bd8d2ab70a8eab21f48fdd" },
                { "fy-NL", "4c3f30d96838a089ea1b16211d8759ec72cada8353357ed7dadaf0034a0da27521fcc115d84d87af80da306feb92f6cf0f280a673abfe7577dc9e3be67f7b4e1" },
                { "ga-IE", "4839af4527dd24abbcbb406b54f1b8c1171f7378ba4728cdbb411bbda7e8ca677b6244d3731cc1825f00f7030cd17f3b2ce96511819d7466046303e32f70a75b" },
                { "gd", "6fb6e13ebea8d9869966d8bb777582d89f8aaac11020fc93726878bb6ae1726abd1d881dd8078310dffe97b31cee546f375c21ed47984f317e04be53d2492b7f" },
                { "gl", "55ead802e79e6c6f9cdc0f1c9b217f326d0f46e2b8ae0a53edcc53887a331320ed244ce62e875e0e2bfa5016d92446762f5b52931b7f74537f8b6fa82515ef57" },
                { "gn", "d2544aee8360a884bc4beca2ea559f419d0a4b784d54e18e98695e07b67f587bfdc9e526c3222c6500f11b8c7a65612e1beff6d55845021ed946ac68013d3d7a" },
                { "gu-IN", "f01204a52f46d4b76b8df3fb63b8cb41991ecdfc6a4b5203e057f4fda86b341df3d8b7adc5e4c5cd4d6fd0093f760831abd7d48be595759d3ea329809ca027d2" },
                { "he", "83fa916486a7e9923caa39d952a44f023efa5f9a62630b3d2bc07e8a4bf963a5004b9883182744a145074453aeccb90dcb67a75ec2d821678384b8ee722c4ffe" },
                { "hi-IN", "7fd3098519208aa717a9d5d5a6b20c6f0e3ff84159ba191669328b162e908ae8e0857ed0f41c3a3113fa4e771530700a226a43bc3e97305f4583db0e44875462" },
                { "hr", "b6b94d65aa6db54db0ff24dcfc21b65808b38801cb8a27ac8aabe99b1d124b00180fda46e3184cbbc103c2791996786b8468449171c9be64dc739b77f09b7ca9" },
                { "hsb", "336a542923e9b00d51b131f7d49b310dda5a7368cada00d1530c3c38720b5ab333ddf0e5a1bfe1b4e8d159ba05706e6d107bfa2b310d9120cc5cb3e959d45328" },
                { "hu", "931c4467c2d74dfa78b473a81767e27cca8633a7c869a498ad6f8548b5eba019b24fdd61bd3ede91ef879961d70ee190eb46f7fa6300e9d5d4e9c56ca2489d37" },
                { "hy-AM", "066cf3400d035eb2736ae5500ce4ba42172aa86d49e0b845036ef2073d1bc1be6fb8f3be60f049b0fb19ca7bfa4577765cb8adaa9653fe4cf0dcbc0c2b2923a9" },
                { "ia", "4de233d1b7894e46be6d2b0f13cdf1d8792c0343804ad592811fbe0bc62b57748cda63222ec67edfb6593cb195a6ba6444e31f9766820b7a63725d4ad80c5509" },
                { "id", "dfe0921defbe604b5c09ab1d5324020c8bac9592500b45e045ae5b47b3c434c1129929feea327532e884486365341a3af11a836ae6ee768577a0b773f47a33be" },
                { "is", "f8ea424da0933635375574425b4b86620e5906357e8c9fdb20ae96c74221d41529a3a49834e6c646112f3b20a3a8a482880350efd887cb1f193b5d1cb21eb454" },
                { "it", "0bb10b49a9bc44a785735fed462de119615392bdbde882dfed8370cd3e1d2839620328c852f8932f1ba336e76d416c7b81cd80a37dfc9cd49044fcffafb34eb1" },
                { "ja", "b5e3fee30918a7c38521dcc7f9f78848f862b7cf28f7744e4156bb681389690700056d1515ff2b15a2c8d639dad7c6829da36d721b6fc9a66e175dbad0d1fb4e" },
                { "ka", "4dc7e160e6b7a5461aadccd91cf8a228183d8e85211478458c05936991fe97a87afaa16d8beb3fd451cd41abf1177bbc6099e870a8a1e5e09707aac9735bcecd" },
                { "kab", "0655acdd967084673b9db75fff7d1e66737577c39cfafa454210ad15ad00d567810b55a21f02a7db5e1dd42c537c392f39a5509e80ec996b6ea28c96bf38853d" },
                { "kk", "03445647bcf11693c79b8939442e86ca2753893fa0955eac80248aa07f4ea2a0a3f6ec5eb42fd1e1db113f4b9de19ef0d7028ed6d5c278ef0ba6a6c26c8db0c3" },
                { "km", "87cf7f4aaec7447fb9002f5e0c27e44b0920e6c7397fe76670bfd94aee6e84dba2e16860154752c408582be9b04fe0a46ef6509e94e7270fca69120b110563f4" },
                { "kn", "215a6270361e7340a4a03261ebc0abe10a3fbe58f00838dc6c1821d5c1e9749a2676e672171b99671fbd064af074dac56e120f8351b9e6aa1595a50c1b031f9f" },
                { "ko", "ab5f3aaf3503fd718020bc153efba0c8358dc98488cc62badee9fa2b4cabb2edf44db3c6329b8d722d3a411b739c9af847ff8b680c64141db11ef58ce375f858" },
                { "lij", "36f367a5ba1397119df5e09c78a86d70ff8017de13acd6116a0391a8712e387a2ea9a8d1dec58b8aaee8c9ce5e7315c1f8e066bd8035b38b3f4f67127c93f15c" },
                { "lt", "2da7a68603b3f66c174426a8aa97d9d104d5226cb85a6e8858496eb1703544e0f88d094ec83f4106e162e2064dd5fb6af818e768599ab9342e28014ba02caca5" },
                { "lv", "aec180943f728d41a50756c0138718d3bcc8d826ab2a70d8f2a9979e96df4db8eeec3e9c002de59c495a3a852d9a600c304ad5515d4dd48a254c96de568a9aeb" },
                { "mk", "d7dba10b870c64e3b90b383b44f4d478cb6ebb30aa687a290abd419dbcd0ec0d7dc201ed33d90fd7984725221f16574d00de1019ba3040fc1f9aef5a738ce2b7" },
                { "mr", "e8476bcb22e7beeaf6623872fa6e3e910ef108e693ab15a0de4ff3f13096cff6c5ca83b99d11e9258aa0309cbbfe9a67448f7890fffa8812a97cb0226a2c7d06" },
                { "ms", "4962c7bd054ba88f777c7af9f5e3ec923df73c5d7d300e1d814550af8c18ca68856fd7012b61d1b0c9399cf73ea4b29f74a91d1d5d34b50c419c0ad35627c91d" },
                { "my", "bec8704f7b528c86821ba017856f5cc276d3b6af6bb7d5a9d94da40e188c083df33aef862f13680e6ef14a38f9ddfeaf9e2b8c97650d7c60577b5d24cc0a04df" },
                { "nb-NO", "7518f1cb3b1f5a3c1dc54666e488ab3a2cedb973ac736e6cea9ccbdf2e1d2a9d156fb258315b198f3939bcf4754d746e91b6c01bb6ac18304cbb90581f7acfaa" },
                { "ne-NP", "86805c671e6d094537e64aaa6657f4e4962e0443fe71dbe38ba7769f2acad14e558071436397671ea94181c1a587429ed45bff053fa354af170064d6eb4cde19" },
                { "nl", "48f317b826d0431724290e7d206fc08df875f4ec87bc0d51107b132c1ba01c26c19e238211c6067e961ad23f33d7c635c48d66e6d589a39cd6273413fc5d0e1f" },
                { "nn-NO", "38254ebd280df21adfb8bc43da2c864a49224c3e48c9e72744022fabd13004509f8dd16a48722a6a05f8a51a254d166e4d48baf722fe256d601a35cffa8d1c7c" },
                { "oc", "4f2320fb4cbe7cd923e54b3f3a0b099364d9362db8f4506b58161baab7200ad645bb5b6b254cc6f51122c3e72cc1c87b6ac3b772149a68fe5da42551431a1472" },
                { "pa-IN", "36da35bc6957e27bc9e8f8e64d2208bda9b68ee841d757452a39e610f70ef33c9473df8a4353078c2232c2eeb92fa510e79fcc5870c43b518521f029aa503c86" },
                { "pl", "8b90f7de2c7ff23cb32d089d09d643d8f064d67fa412bd8a1e31756b24a62961921f1cc03b94093ea7aaaee6440bfa76fa135c68eeedeb66c93b050b4ac1dfdd" },
                { "pt-BR", "c423a4c432aad2382c53366ff8284c811127077bcdda8da187c862dcf66ef20d0ab48171e661e70a81068b97e23e7854e4babcfbe95ba46f093db648e1dbccc1" },
                { "pt-PT", "f60b75e4731171724965ce7e78b7542c54558b58d5b8df34270a6158ad2fb57f4dfa207376f7125781ab3a09f55ba9858caf87e955e985c9a69aed5519c208ee" },
                { "rm", "0bcd69bf3b8aac4115186452e101b8c2f030ecb217b72908af9837ed3d0336f7471ae7e6e81affa6319e0c0317f0664c5fa8319f8d54994e5f6c60cea92995d8" },
                { "ro", "438620bcb6f544783ff5876b37d8fa828193b173668397cdf05b0bb7048aba1fad0f3227982ed0ded8d0ba31cd05f05fa63f98c88057e4c0291f8a6ab645cddc" },
                { "ru", "256ee81df54b3dc857bc7a3340d602e9cc0ad8b3ae2292f516916a34fb0ce8cbe86af2f9af37cfa2d14df607e72d08ff770d1ea0ca1ac5de60e084706fba2de0" },
                { "sco", "be3733712b9ceb24257d8ce6da61f438306a2425a84b27418a3d4d0c4a530fe95f94a907beaa9b6d92ffc48edc8a95a6bb7d83d43b579ac91379b135718947a4" },
                { "si", "268e5797fb45cabd76a422bf7e6672855f51a538998fb8d79d10862ae29db0b094ad5799cf1b6ff57a5c768dc76f6aa1c025111606d7078a653cbc22f580f00e" },
                { "sk", "ee3fc6513b92a666f9ae2580ca1386ce0c5b1f67098794faea8019ce71b7de875d357a48beeb2838c8b5479a7f2e71c99eb6d2163f6209f6fae959e0a6802caf" },
                { "sl", "011a02b775bdbde76a81ca249e4f458a505f14e22daa7b4722e4da2d0f7ac5e1470be7898772f7a2a943d3780d71aa5c4d159ca02001f1294b55cc7e61640ad1" },
                { "son", "8ef09ee599df8abe47d58c1387b6092c05037c36814d5f1a2532a521a88dfd8f857e0d1e2c69577f4eed1ef303a648aa5f03943c2d9608e4776408e38812c9d0" },
                { "sq", "94d4162374256ca730b0a9172525eb371bc1733a99763151a992bf2ae89ae507936b9de1fd3f2ded1da89a1c808d324151c28ccc531fb315841b7f1f1df7e147" },
                { "sr", "20220121fadf95863c01fe6650a60ea96d32750b4ae59d81b2ea8561c1d305631d554fa7d7bf90fbb61d0857891393f1630ac8a0516dc90ae40d5009026876cb" },
                { "sv-SE", "8421c29cc6841134ac6cbf3e4dd8bfeec2f7013c64bfc0cbc88a250e9a8e8d9c5bc741bd4e6b612078bf91b58320a090dc133ccae4db661e044fefcfbbedb6de" },
                { "szl", "9074712b6de7bb190adc9f5f2d0e18e48140c12e1fa31710e6b75adfbfab53a37fb5d3566d0d8dcb4d46685adf841665eab94c92b2e9c38fe5893c7a39a7f8fc" },
                { "ta", "b7bd37a9197f2e9cb6870361ba41feed3d8fea676eedf03b4b6235e4aeb4977fe6dc0105f284069355cb45f16afb2c193abf6b67678ebb19aa47739c8fa852c4" },
                { "te", "29ad4d99eaab37612214399d71ee8537f2adb5f79f7a299f512e0fa84c805e2c1a13b547e92a86dd47fbcb09f7b1c7f9f2baf9ca25ed3c3b7475821cf583e922" },
                { "th", "c7b29a74c76eeebabe6bb6abaa000b5cac428e77f077c98a429dc38a41bb12e18cee2d847a420766a8614e86dd90123085c2d7be1ec4c7fd6823581e6ad76728" },
                { "tl", "72adeab77bd1788b645de52df2c945f09a6ece6afef22715205002ca1d818136dd0c2439ccf3e3a2b45ecfd95902106103616931b330d67f372a1a8ed68a8296" },
                { "tr", "1d0d47df7f4e444e6eb405e2ff9ba77db11a0751fc34190bbe2f1add1e1e0942a89e396b8942066b42740b411834a8bd9faf5105629a096c87f7f1bce3e904b1" },
                { "trs", "82fa1e261de58c791d79a77dd2b96012bd1b715b72df5af50626691b3481eb40a6199a6377cd89dcc7024a045961176a6064aee98bc82aa1b3fac4265c78cf8f" },
                { "uk", "a9153e7a632f05b66d657daf9fbaf7b5d48a2ba9e79b8ee2d50f62d962c8aa9ad2a58b1c0fadeec427721848f9a98de3ba974c7165210a9c24074f558458c629" },
                { "ur", "28cd7909b737527d67462d1b89f9e08c6830f985ab5a4175926abc28a3bb143819adfe1d091e415c62bec16d59d7b308b734cc3f3306c07b7c3e87517e54b926" },
                { "uz", "f996950a9cd923cc364314dbdce5ffe48d79483952ffba3707fbc2323cd8f3a2a744ce75afeca2dad68a5abace3ea54c12bf403d8a63ad69db4ff06b03619899" },
                { "vi", "d2fe8f480c14bfe7f9c25b5bea3e9baa7c151de23e8d7db01ada5dd2c74ea0a72fc970773e628f88c5a94b4ded1ca5f931977b13d8c759b5593246e9cb4ef301" },
                { "xh", "a732d00ebf5a86fa22bc4ac0dbbeb4d636faf70e656fc924cbebbad9a1f3f4a7f75e73dcf37e330c575fbc69d6f9e40e164665b57a94886f0a85bc2895d5dec9" },
                { "zh-CN", "15d54308b34ba2657e3588d154506eea28996ae19bfbacdc3e53b9169abab38016af7a8a6dc51dee96dfb9e5d38033402007e8ae5b43b66a6ebb27cb11e92c34" },
                { "zh-TW", "0b9d9e8f77483d17c09bf615ec2cbc724b764393123d57476264092fcd46d373d4496e657fc8d1e5703fe1b8cb905b3b639e776a815aaf7362ae76f9eac0e755" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/95.0b10/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "1114537f3c51b0abe004395d741367ae7d18345d13c6fdce38fd9edce850a8ae33fbf64689568f654cba7ea0bbe0596a7ec6b5fd410e12c08f09c0e0f9533b7b" },
                { "af", "9ab3f385a8e02afae7133b692262a200563a0f5a9d009dc6b9760434ec0f174f5850a56c6a1e655f4d6efda422df04f1606c4b2fd2cc100c845bda830628be10" },
                { "an", "b984e8307c9637461c6d73a9f09ab335bac053b2cfd295d518c7e57bc514e330edc8a7f15efe9a0b36558f021b6d7a1b41cd44848b7e9533a0feb67ff131b7a8" },
                { "ar", "859e5c985a4b064337aeb27c8bffba9a0436073895b561767e6e0d69d50d71334ed7c0a93798a698b3d6c95aae712654d7ca97e2c223761b8ac41839f758bd3d" },
                { "ast", "00d3c6501ce0e64810f3a3f8b7f46033e1ebea030e9a4f46ddb83f86860272c38991c5e0338344f5b1ec53c2b726c70d7e83c7c4dc1b844103ba00b27105954b" },
                { "az", "f5f6af95933c9e32f839576bb29bbdb21ed5f6698986bf3c47282656821f2bbb16522c03631e0ec55a81ef69204496709a3aec68ebbbc2bbc1c4d3a3cf42310a" },
                { "be", "88df4a5968d9ebfefd1a19bb48a0dedcfaffd62e901920d1c535aa9ea4fe9706025e3907c797bd1a36f9c21e92cfc276cd0ee7e8ed013a1d9fe875a5c7a64ef6" },
                { "bg", "8d83ce1649cfcae1d3375809d3e695ae357c07bd00a114b0437313302da8125652ffb8766183b9ff2bf10705656fe11612b1c7196305350f75f796a409d59a96" },
                { "bn", "d3b172d7198f38dfbc245bb5f9b9be5a7ab846b0f35a32a58cb72e01758088a4dd0fef9518e2dd1ad58993dbaa93af1a15c79091d44ee66a400c1ec4e4699696" },
                { "br", "c1dce6730dbbc3103f7f3c07b9b58e3d5a38d6fec903c029f0eae1118ee1b8a3928ee1bc37a6978e0a8d6219fdf3b5a356144e17aa49165b0adee1b8e7c2fb66" },
                { "bs", "ad8a181a8174e04e1e4bb79ea9b29226c7e77ca7e95ef21a23bd54198ad38a509c74b1c452224f7a71b13dc38de69c6e7a8dc3e1b6933c3de4b2dcef3d4512b9" },
                { "ca", "831c9ad992512272ce6b8d170a4cf01d2cb74960a4280c5c153db55084f8339c54022a177134485330d58e3d50fddc39d49d4f398cd30d64e98e56e46e6912ab" },
                { "cak", "28b27e799502f3e767f337ca22a054babfc4660237702b5fd835d1ea1c5c9056cb031e89e4f4f037713fb0022c5b4a2260391dd8c000894b9ff814199a5b1432" },
                { "cs", "d7ba4b99b422d88af6893504baa1a2dc758579f0eabf8afa2c218cb319cb9fb68109160adb67ab14893b4795c37eaf2a99257e89888ecd0e0d2fa3f3d3ba8adf" },
                { "cy", "d88debf8a80613ae3f0397a38a05e1f1db047ab63d2d3766559947ca8ffacb6e3dd172acfd3c10c4fe4809dc385194c7ea0bd2403d88747ebcd54646e5478c3e" },
                { "da", "1d8abd97fe4bab01acfe88bde5d3d25d199312371ba93c5583738cf97c4542fb4e0bad19af8722bfea39b5b273827b3d8176f9982ea7ccf4ad596badc9178d33" },
                { "de", "c893a59c2d9216496e01f94006a54c8607f98f2b64712844d225fd0786276987f71760ddf4891598ec691d51a2b85dd818f3a640c220b4412701c716781a050d" },
                { "dsb", "e791259ae176257d7410834cd99e4e31d5eff036186e7badad25a78e29f149eca55170819bed160fbb04af5ad1da689dfeea86e41a58a78521d5a53d0c1257c5" },
                { "el", "88f618477bb9879f6cdd8c5aa052b11b964d43026401d258405f26eef49c72e984f0a412b7fc7743b4f32fcb0b5605d9e5bd92af2e59c5b3f262fbf4d843cd6d" },
                { "en-CA", "81aa63e48d6505504467fbfda10f1751752dfa07e2ec56768f57faa92fa74d859855ff4b350d9bcdcf50fdac7d163b4e4ee6b31ca28d27bc643df53ac7f24c6b" },
                { "en-GB", "464de787720ffbd1ff53294ea1e61a0dd2afe63e6ef25271f304452023c2b5de167f374b70a4b8f318858157da5dff5d3d5f55d71f27a46e0a5125b93dab7a7e" },
                { "en-US", "2f12c69722f23aeea734b86c4ad66a232b5f33bf5d02d7b22a407e7f6a9dada1eaa147ad87565156357db56630f25b86ab4cf01d89796ced41ea2fcc19b0a2a0" },
                { "eo", "e14ea10f889bbdf8baaafedea3555c7c0dc439fb077871c489a02aa5c4542efd65e66d1bb0adc455618bbb4e126bd0408d647a9f64295d869a255113fe431d0d" },
                { "es-AR", "80f1bdb8d72cbac6cc2276dca01c5d1cc947d8b352e81bfbb78e580915c53cb778915009a564e5c1392d15b46a5dc0302903d76aa1b352e21695d230ccb16dbe" },
                { "es-CL", "d569ea456cbf97d92264075873cd26a3552102da6e9b80a6bf86107f55c1e2eb03e4cf7fbeb392ec34fce6bffa77d52b44dccf45791da3b6fb84e25d15b8f4a2" },
                { "es-ES", "0ce2d7791b97cb25b352c629f51f4fa3db75a79174c598fa87f882a340ab0a9995f586a35f41457436796c96108522f06472371325c36d46eefbb4c5bb94ef33" },
                { "es-MX", "c34617f934ec010f5f4d7b96f86be6f6ea893ef21b38d0cd081b0041f14e4285665f31a9d295ae01a02fc82866f0ba86a0380fca46ed2b70e1ab8c0026d5a993" },
                { "et", "15e5d57801a40cdac62f13b7f4926b10ed6da82312d7fdbaaf3bb4a76c00770391d29e40fb1487e63d48fd906a8296706090dda5ac012b97934a6db5d313bf04" },
                { "eu", "c96d684f7e03e5bdb89c333a847aa4aa3fe82a17fef27d60e1345557772f9e75b32c417ef6176f5cbfdd51c0436ea4a964790ef374e79bb93b62d12d96ecbea8" },
                { "fa", "8a990c16ec855fa0f09a0f5e69111bed9be61f528b5863188195f887df6a24fff351f46b986c0f2a790367051e88dac05414a9ceee9782fb3c0e79ae977db906" },
                { "ff", "e1e485731f6391c25af6451b26b2c1a8c13626b464abc6160aaefe1798dbfa7b6778cb971597e4d68d32e07e1478f3127a26099bf430576aea7a79a15bcdbb1f" },
                { "fi", "699452e8e06e4050554f299d8c164bcba795e66f89f30832bbd661efbaac61e3aaadef88b4b8d78cb6de761ef22bf3dcba12a79dd46e622dd9671fb4de5bdc96" },
                { "fr", "10564f2091afdf6b7d6e6b100876dfcd6a03e363a11a73f0d5a259fd3373cf3d3fb24c93468613bc4821b5bc78e44b4737136c267cbf978be74cb327326e7bb8" },
                { "fy-NL", "635fbc38c6d95e3147a58796bd8b267c3fddd231ffd8f518da3b6a5ca0ec27c097d462d70f81e1af3443030c1db39f1f9e11aabb5d44e07463e7e87c769101ee" },
                { "ga-IE", "d3b7b575ab01a0d61cad907d2748fb56d7cb33bb07da975d3512c97bb40ec6d64070ab018c7d23281b7891f751f71b618f87bb642d1a15c1647ada621fe74e39" },
                { "gd", "f508ecedbc6db8169a7483ed2311fc307e80ccc22b157eb722383a2a6667a246542a012994e8c9d85a06a27fab245e2616e49e872266456f8de62b36009715c7" },
                { "gl", "42f1198b7683bd0e8b5f6794cd42045878852f78dc3df7b29137c6116a0a66e8b610bd997e054381f64e45328a59739db07373fb5f1d687cae913418590d22f0" },
                { "gn", "f0f6bc44c64a306d1f09aea8251a2db229a2acfef361294a5432f6577842b277fc88a732cf05620c68b8d4fe3e067dbb45456984280f6791a9b73b5084f64728" },
                { "gu-IN", "a06515ebed05bbecef0436c27420b824b0d958d3251dfa4b45bff890c7df28ba19bd3f03ce875f39229a4fb5500547d1cfc149730797f729790a41ad3a923faa" },
                { "he", "bee5fdb578f9a9343154ca1781556e0ef311551157adc4c03877781f713144923c103c293262d8cad6ea551eaf366beefc2e6d208f46d0edb45b8834218aef76" },
                { "hi-IN", "9ed4707088736040cbdfef594a76608ef107db2eaff210419eb04747af3fe179caf21a6a5a1416268f29e351ec8c6b8cbbc431101b9301dcf436c0459b79aa8b" },
                { "hr", "d2ff7f6555f5bd7c5a5096fa1133643784eb7734e142572eb67b896ada1e4f21d1219da70136e330755ecad28fbee194999bd493bbc75ca720f655529444321f" },
                { "hsb", "6401d87842edf402d0c3821c1cf9dad86ca2dd11d7d20a0ade566d2437e3f2d8725353833706f40aa09b19711f01a210d84b3c8703533318454b44904df97205" },
                { "hu", "585d0242595a92c44bc4005d87ac3e0c9b7808100e1a3261ea017b8b6fab8102fd4300339cc3942ac0e58f8f1990a2f16a63059e2874f2a8129378c2ab1cc457" },
                { "hy-AM", "07ad00bbb9143bc1842381905dd254d543bb42e89dabc2af9050f899a993c7911b966f113ae8255aa0eebfcc5c423a43ef892f50891a9b8076b13030ece5c9e2" },
                { "ia", "536f9e6cc8906e5dabcbbfbc6f41f0e51df3e2215f84446aa329f5330c8905b8dd9d3d924b414a24976e9956a17207edf5dac52b9ec05f8e188384fed417a8a2" },
                { "id", "e524b8ab84bec970bd5f0d9e6bffda37941a77f67d65b62adf13ed1ba28450b69e0d18244849cf6fdd34bac5b28a52f1c22aeeaaa45899c2c1a002e77d2e5bdc" },
                { "is", "47f076afe728aaeddc1bb3e95307960df295e47c46d3e5725023f4fe33e8ed910760cce848777da4e3aa7faed86b5f135df20416ae9a730f570a9c78d9fd9b1f" },
                { "it", "e4dd697611c9c6cf339d8db43ec1f84a6e996549c6f544d80e501e6708e322e76f350c686086e07e74577206dc6e1c58fd5e39626e0cab1091fa35f4201664fb" },
                { "ja", "f5eabb8a284f80047575ecef9d8c72b9931b41adf529bb149d40c333839f78346eeae1da68787539064d353da68b3f150479ed02d4fb05148e8f031336ae9e1a" },
                { "ka", "d4d067a229805f557d089d1d770f5a4c6f65501491f9f181d9506345b2f150096320086f32e3cec423d18b1498544cc783699a6c27476729f8f29cc798cca32b" },
                { "kab", "b8e1f921d638a073ac6f70d52e54f9b2c01a1a4179df6caaa17a2a51667aa5350e69ca36a24b9cb8f2fc5ebf8f48a4812e830d47329f35fb3270845b3a753454" },
                { "kk", "0a32faed8753f317e210152dc6199cd461e3bb301c7c25bda406d4bdcf9186aca2c91666480f2ffe2bbe7fbac4a86fbeac86f7ffb5d46827a046b6d57d1ca54a" },
                { "km", "23bc5c6479f404a0d1dabb405eb32025001a21963d79ff86d7f180502af5209e2768a071b64788a0104df747bcf2d1659da69b7eb8af8b069932eef33fab884c" },
                { "kn", "c54f5b7bdb2a6a625fcdbf48758c947975990f22046841208f4335810e6130e145a37cdf0b6e36f325527ba9754036d0e5083ec82232948ab7c99c60225adf03" },
                { "ko", "47fb23d665c99f1eccc82a411ed5b95b94c13603a4b2b471695d32225480edc9a8cfaa2c6408a2d9954b3de2da7053b1d61ff1093774063999a9fc4b9ba2870c" },
                { "lij", "b596b7fc3f2e39ac7cf37b9922765603b0875bf457c8b55729dcdadd781914e438019bf46e38b51f1b2cc5a2f897eec49f7a2cbe2d3888dc012557258698967c" },
                { "lt", "3c2ffa65874dcf1107fec52530078172ab7e3739fed0c566e74fd4d1eb4bea54d85e302468daf10ca73a9a192ac005db05349a904fb813ea9042e2fae7913d2b" },
                { "lv", "cab88075205dd773e7cf02ef030b31c9e84c2cf0a43d264126c30092295735d2f3bf78dbfed498a087bc70bd60dd6af10a73c7f80ddb27a681ec5da7a6ac4937" },
                { "mk", "addf43166b0dae32efbebc650715b3f2570baa9cda39f3170a5004c0366348ee1f97627fff9fef5a452dc64fbc4ae1bf39501f8624e3685cf35867bffd560152" },
                { "mr", "65b1d8be854f634cf385ae230efacb67c80050f171073ff511cf2423db31524f6a153cc35d5d3954c5e8a8ceea50c9920fda9bee3232ae7ddceca0423c19e7f7" },
                { "ms", "e8de2f4421cc0b74fe4cd76dc9a5c2c96546125360b2a44d7658db28bad0839683c435e7f38c6ad6abcf94a734426abfea5b19f5412dad264d69555e1bdf7011" },
                { "my", "c6de56381cf3badf0dacba079ddcbb802f115dab91c598dbcb168482348f03dfd59c41038736ec07d1132c240c4720b1586b3e1a1cc07ba632daf31f60ea5d3b" },
                { "nb-NO", "614d7101c766dc884630f4775ad8cd09f3f2db89986ccd99b2c7c41d62ec9211c865a61dd4a323380461c0734abab226086a0889cc7ca66af04ad182de44bf8a" },
                { "ne-NP", "113ab3d93987fc0b18c9aed6e72b8a8fce25ebe6b7e67274e1343411071afde1f832b3ab39ed3701bc44b3892a9e506b5a41c3e371390825f1faff52a46ec58c" },
                { "nl", "00f85eb1350e9a99cd0af08b760f76d39e19256f89a39e4fbb303eddced3abd535f1d3127dc03ea66960332ed0aa49db2f6718a52c7cf2a5ccbc72ab3000611c" },
                { "nn-NO", "e28d41dba6525f5f0f48bf9614f5c6ad868a706ecbc163e09e06bc5373e75f9380b9af9d779d5630a6191a89a451b02d4333c43702e294b0c4ce15ae939abd0a" },
                { "oc", "f1eefe0f867b5d7646c57fc7148efe8751add52e59b5c9e7fde97d2bc6b1c7ebf27e6ea8a286a9f2471137aefd0da722c8c794e5d61d46afa25b49787df8afdb" },
                { "pa-IN", "64e2919d03389bb86c75565d2a675bdb0cf6617925af7fa5b3d7d105a02d539122012244570b8a48c8e1732b32a0e7ba354e62832f591199bf4b08f518309abd" },
                { "pl", "d8f040a4711fdcd479898dfcbeb3d4cc3213bc2dee07da1ec5c9399619d8ea80722abdd911e8d6d3310a79ac5d8f3f2e11cc85ce7fef37b06e6b5fd007c4ab09" },
                { "pt-BR", "9ddba912214511d4cb5e6f83752a28ff71450145fb01d1ce350c0f58955f92557e383425b3bcf397d33cbd696fd7ec5b9e78ab704503fefab2247ac2d8877792" },
                { "pt-PT", "4c4123b57bc8ae5bcadd7771f22a2da7c82a96ff2a0de6c309763f4a8f0dc38a6ab9c7be41fad506b6c8b8d1589d15fa1eb6078ad4e5dd5162605c3048db062e" },
                { "rm", "6f4293097df830719dc4ba3fc686daa73405a8fe4171769678f9df3554c47c8c92afca83c2f3f3d0d73441db5904a85df2cd4ef0edc982092fbc1ecde7b28919" },
                { "ro", "0f2cb9279350d6baf14593f9f2fabe86bc90193bca03049aeb6bde843f47b237164124e7a356ef31e399849f5958181e6f7a4a5177a8afbf56b856159b2fd62e" },
                { "ru", "aa643913437737b264bd6cd45ef1a879d905ad466cc7bb754e6cc3dc85b81c7d1777aba859e21c069b62816f389dd73e54e68a9d874defdf1be0d38b85110d1b" },
                { "sco", "0a1adef8008d7a36d03fdf656b0c0da5bcc8bc55eb9235a3fdbea4cb113aab42c18eb8a8d321ab38ce53327657af73e9b89eadf7ace3b45bbd5352fe20196e2e" },
                { "si", "233a31d94d78fec7a7b6861c33711229b05ac7f341afe0cd79153b3450ec5837b33d8f3bd4a0bffc75d901e903ef5939d1dce28027783ccc1276717c8b7e993d" },
                { "sk", "f6d47f3a0108274b4502ddc8f96f4f6bfdf15ffa62b3f3870d87b7edc3ad73b574f3699dc709a514c93e116a47b32c0ea3e8806dc7f8151e9ed5a9d71b9aa56f" },
                { "sl", "c3008bf1adc914c6d8500031916e40dde36ffb31baa747d9e9632600f050f7565dde46913b8b4fa23d3b3f52080a43e150bff00261225819ed6f263da26ebef9" },
                { "son", "e7a8393c8ee4733a304cb8e7d24cae453552be513aba2900683689ff0de48d49930004d0de3c34bb7ae29d912f0e8f3ac7461373f2c2a353efe52ef38195decf" },
                { "sq", "6870889212c5a9c26a4a5e26e4abc033a2caccfb2224755f3646fe7b4a46d440c57c16f124a0d505e7cb30ecce6ef38565d5df20322616abe8507f76f03a321f" },
                { "sr", "3b55e20f30d961f1c49e0cf525543174c128dd88847148c55d4f03daa2f6dd9427f310318572bd561553b632fe236319b3570b34c31ceac6dcbb46894ddd0cbe" },
                { "sv-SE", "c3ced5bc4c864b17e2f9ea14454d41a3f3f191d8f329b67c5bfdbda3a7f691b184514689ff6ab8962f4e95fb0619a690aa182c6322d790f08e37a81d8b436e5b" },
                { "szl", "677d857779820a261384cf4f8991c807a26b0a65112ddef77e0c9a430438b610c4d58b67b31b75d29363665e2efc26f48919a227af1162583048f6b192ab4b4e" },
                { "ta", "75f5150c762c0920e67a2888aab1a6518b589233f7096686f0328c3eea04eeed326af2f2d11e509eae20f8fbef1500e09a1e5266629c156da01ff445a4835998" },
                { "te", "6a2464436dbe93b46417b18d9793001bde4f675191158ac8200bedb7b67f6477b8e9aa2ebada42f329310352bf938489dab22b607de9f05d4dd73ec6e3ad453f" },
                { "th", "c3d658db1d80bcf03fbb09fa704b24b0587d0a88262bc2e346357131cc2bc58c7ad46e12d7bef976fdce14dcd29fb2d508af57771fc5d2c6c4c36f75737b3e42" },
                { "tl", "d22371ef490f6ea20c77bf38b14ca409d17cd8319cf0ea2fffba42b11a2cdd0764dfeb4b370d8443215bb8a55644ae1958431295017bd15b65260b72e3f6527e" },
                { "tr", "c5b819de12d717d0df8749ea97d755522d525c10f7f9dc421d2fa095ef716d266e0cd546ade5a2230f5730f0fb97e91423ae6adc3ead3b9c8a90b7897b5d3e21" },
                { "trs", "b8be71939b65f82f88022463ee8895bacb6d3bc1e2ebfc7c255953f14be6bdf0a67def66999355ddf1b4ee7f98ec454de03b75771811adfe2cee51bb7136fc2d" },
                { "uk", "a5f34b8de84bfa158b4bd2cc9c0e7d211957a4e67246f94b1915c056befd6aeb89990f29bdaf4e26ec7c4619005602fc0b78fba4d5fa9077a8a6426602df3dbb" },
                { "ur", "e2f903e2621c665f4abf69656e5284e6ec27848b5a837caa646eb343532bf494ce4804ddd78c3c49f5a8d458e09defd28f6874fcc730547be7117c61ee714dd4" },
                { "uz", "05d6459246b0b0a0fd4866a478018f63d3410c48b678499f92a44cb389f50449e51f4bdaaa2f2037b947e0cc609f57753871e9e2e23ce4f62d5e90a6c2c8a048" },
                { "vi", "dcc373baa04207caf897cceed1c517bdc48cc0614177ad6965d4a3b19c492ccfece7673e74c0e233b691194c849be651ffd2dc81dc78a5f5ec4f0a9ee27fa382" },
                { "xh", "e53a22f3610b9189456eeda60b2f19bbca4da99787b35f2caa1babeee7deadc9796eebb4fe3a8d0e20093797cf311b127d0b2d5409bea874d32ebf52f4d5d66f" },
                { "zh-CN", "9059b9d084bd1887bd6dd9df79bc07827cf81af253b2713d64e9e2bdcc8ca303c5c35f2f461ebbf8c6cb7032f00b04da675b76ac09a78955b27d6fd246b0a36a" },
                { "zh-TW", "d5ed19b70ff1e7fe9add46902c76af491532b9568c508e747c6cd001667190b9791e4f227e03b694b7c503240bfa62e5a83f5657f1655e2f479132161e016f94" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
