﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net.Http;
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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/129.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "57cf30fc71ca35f8914309b612830f940e1922dfd3a358ad8f93a592ed945b94b94d3a80d5092174060bfb6c279b64c3b6d409105738cfc4a7bc5f4c2c0a09af" },
                { "af", "15cad8d37028c49ad2dc83c6bd1c37ad36f94c0795d566395a7e1fa630d27464b01da764c933f4b422a67240cd6183ef599beedd2390ed3243bea4748d90986b" },
                { "an", "2ebdac8a044e8bf7462b961bde1fab56bae3158a0d9d605bf75096d53882f886e09970f03a246924b6fb842eff9cb47f44205e5c692d52de4b208c9ef04e5b78" },
                { "ar", "616119d5b76a61978085d53091c5a0dbe31655c82a734b254e7b59854325085a17f6ce0f1b5cb0c7bd22745a72a4b89fbaa41a5a6edd0c50e4222afcb6e4d99d" },
                { "ast", "b5e03762b98fe5d8da627e30c6ca2f39a99692ee23af18775fbce9b3f3b81d6410d8c07199cc0a91865480f3ff555dff2043042452e5fe4179fec125050436bf" },
                { "az", "637a12b82384487158037ad7f651092b592bef6cc36d665dd9833cdb2eaccea6442a3b9815ec26cc3d2239ff6677dc5f724bd64603da9cf5cd0de25d18ca37e2" },
                { "be", "4200bc81b654149599820b9795d6863c41d808f32c044c1ec33aae5fd372d7b2449447b8521c4acbe318538d8d651323e2eab43d92914cf3dc65f63fd411d76b" },
                { "bg", "5e5e6e4e789efc814b7907fabef9f73f8c59e3f669250ec1079e85187ff3454b593eeb9fbbd889a6e0611923f66f5f5978ea9d8ea517bbd2cd6ffef936b1a8ae" },
                { "bn", "11eea7da108a26069a597a3231a71695c5f22ee7dadb10fa7cd42a506e30a08f30113a8c67cd4a3638294c67439a9df98722b6bda062b020fb89baf29ed8bc80" },
                { "br", "55506b38a0949028d46ba5032c8db4c8ad114f4caf49d7ea769cbe9662d6f6cbcfc267d48bf300d6aab4f7a0068c11f1b2295cbc334a7f11e5b0917a70a20ba8" },
                { "bs", "40925718d95fa7b74446b979576c7f16cc66e663ef82948279b9011c101c92d6e69105a246b0639c2e780473da0dac965a8424f379a913d1fd3e1cbc24965865" },
                { "ca", "e908c81121238bc0aeb4bb84a274795ca6e07e37ca3bd17c73630dc1f1be5bab723337da2ceb53042a6098b681c320c9aa110960c5de9f6f8f7bf6303b7b936f" },
                { "cak", "a44fae258c0ac9c6a91851b766d5bc2af9faafabbde7e05fbd7d8a1f2678da2274bbea07d2d0587c16e3206ed9fd3d0f4370a61935f82e35567f55675ec4292d" },
                { "cs", "41b62117cb965fb18d287ef8b3c7a5748b2de306e29fa29b798f99d63764c9b15a4fabe07f476d4582e23547124f9709f2834bc6c601cf9d4a6cd27f1308548e" },
                { "cy", "235c385ad682e7ce9f6121d4045aebbc65e2208eca152e8138a395aa83af77dcf8a5b7cad41cd8e62c4377cc2ae37312d30d5cfe910b3a3fe6a8f36352a557ad" },
                { "da", "c0beb536e1b84dcdff96a7ac50a498fc1e855f2b54f1da96a9f6e9de7ff9ca59593f26260c47b6e2c22e10a7b4c2750405ba1cecd3ebfa3fcba3064cdfa1e55b" },
                { "de", "01937acd5c8d15d85939dc4ef2cd5ac8f854a54a68e98f00b394473ffeb77bc552072a7df3ece31a541960e2163375354b90bb8e00871b7dd6f4fab5da56ed5f" },
                { "dsb", "be4e5f9ec3bac0bf5663641bc09d3b6f63b3b4f9eeb39b7fb76293d728915c375eac70957372e50cef571b4e19905ce3f033a476b7074a91e5f3fdb52dec44c3" },
                { "el", "f4bb19411f3bd5649adcc4ee3bdf75539421b3cf8ccec44f39352cb58f30a065cfddfa31438b979cdd9966a5720b89989f395358907a5ad8d9fe602169ba2c7b" },
                { "en-CA", "f05541d6d1c1c070eddf8d1a06c58b1eeec12af9df0d8d32afdf9d11988ced1c481ec411fb6cca3a5f4b3566582d772464b791bb7f30140c8a2a6e945c0ec736" },
                { "en-GB", "223fc4e4c86951cfecf7a32aebf3763f8c25e76f4b4ce023be29a83a7f12124c8a406861b1d5f37a47f05b3759b907b6a2b2c6029159605bf7f8c491435ec4de" },
                { "en-US", "1090acd0085b994aca81d2703b01550f45641ec37028f7adb9b90163850ab519786580f697e8dba149e15efd50d4ca630d08acf3ab808cb1fd5be44de6e77341" },
                { "eo", "63b26543c4504ddf398009dd36afd96031418d9f2932f5301e48cd183fcc786a80a3ae2b08f83686c925635220baa50bb8c6b21feccf7250c14fcdcfe71ddc59" },
                { "es-AR", "9d736254e28a096678a5fc80de87ca770892871d4623eba5b16c79460422ba3e238969d5498f100c9665ef4d7c6007c26d2b0152ed564b93e5c39b76db4a7e69" },
                { "es-CL", "87ca07163fc7beeeec0be9affc804adae0964c705590907d6e58cef85d2ce08a6cd06a56f436cb0d1fb8ec956cba0b0a1a2ff348ea09672438f660427c6e3ace" },
                { "es-ES", "081663044c2066538cb24a34cefaafc39536944cb64e78bf3281bef29c628bf5fb976ed35710676b367ffbc6399016b82f0d2eec68f8be28df7b774fa6a266b6" },
                { "es-MX", "17f2b8e2c55a7c584448ca51ba4bfae3ea36272f36326dbcee52d107dfcfdbf79ff863f7950f99e5ee9bbcc73af6211700baebf84858b22593b9fd723d8aa51b" },
                { "et", "25114ba84be19559a59f3e6ae1c9621e7db93ca88d433585d1e8c9832e95bebb94b8972cbf339888ecfe36980cf62d2dd9bcda6a59e2ad25c51aba5ce256b6e2" },
                { "eu", "2f037600fba6f4c6054b5648eda16bd47a29d8999a0a96e6f4ac422aa227c52ed8e9f5b50950a147ea209aace9de635defed960d23f9dae078afe4290d5bf67d" },
                { "fa", "0f45f9bee1023b74ea7759400dbdd73bd726204adf1e145177c085a2535903f721bb5b3dd89ee13bfc49f22aae62d5b91d13c5d45edc6335d23c965facb2eb30" },
                { "ff", "817a73dd9f7bc1ad6c7faa4071c66f5aa6179ec6b98105b84bd03a06514953a21e1ca901823776d2237aa5afee8752269edb3fa10a2a3983edfc544a13603298" },
                { "fi", "8ee8fcda2dce74cc4b021c5becb2ff01dafcbda636722079e378d0ee76c5bee309313d6bddce3ad50a5263f22a2a2aecebb1a84a082703646220efb2ea6d35de" },
                { "fr", "debfd240dd82fc30c8e9f30ab001368056ecd48ed96cc75ebd59acf0014e6df4421a6a9faf6ea9d6ee2ea22db2c493daedc5ceaa8a214d51291c5cfe2c7751b6" },
                { "fur", "72a7f3257df486ec799c3bbdbd60ffeb15d5461a16be0b8efb6974ae945d504b75b007036a2ff226f0999f3f58dbfcce37c7e794752964dffdcdeb9371a08d0c" },
                { "fy-NL", "44c3cdb213399cda226ca00b429f838917a237cfe5f5f97a57739851011b89f2c41ef1b7033e126b7fdce2d0d9515d07b4e8a19813a13c722e088ee8f4081720" },
                { "ga-IE", "047f75e699a563c56db444cafc3096dda5811eba0c0a0df552d6346617df28443a4af7976e08ec8bc3a339e1d74f1907e967b9814d2eaa1321dac08dfa04be01" },
                { "gd", "a013c117346565f2db937f64939971f2663f3fbae4798604592628bad132e7c47b61dcf1cf7e07380a76fa69f262f254a2b749f7aedbd2828cc162f316a4290d" },
                { "gl", "4318ce1a0b38d3cb3abd3b8aaab8fe8dc8404f2d9d62fbec009f104d680e3860fe1771ccd70bc1ea4bf1100fc7c70564087b9a9ecfbfd025075fbaf0345fdb30" },
                { "gn", "85f6045bdb0c886cb037bcb39256eff22248b7304be1a54726ada37d048b5432a0e7d1d0ad1e2925c6537e7aa6bb43f7b1d02e37d072974fe37a28afb7494559" },
                { "gu-IN", "f8c0b5db8857a12325d95061b051f6f5a97e9db2ae52639e4ab3c96370a46fec419a3b2fcd7a5410abfc6601171cc1182152009d54fab5845fdf349b15d3419e" },
                { "he", "5c5e2c9280759e80f8811f898c8668cd281b4cecda96bff738c5602cd925e050f189f82836ed594e27b848aca698da82a3bcfec0d9cdf04ba2c871b727829512" },
                { "hi-IN", "344714d9b1233458c60365ae5ed146a4239859ffe9ec0c4dcbf80995d77cf7902d8c2e7066949bb33bfc2bff6d9e750a7448bf6fdf5f3abf39372298168422fe" },
                { "hr", "49eb32825399deb7c935366a43190e48bc0cc400ace76ac9cad95295a78c44f40a4c6e914b176e2d0a80667fbf6b205d263cf8f3fba0efa2bc04d4dd7de9886c" },
                { "hsb", "356a7435285e7187e9f715b68aab773b089532b0c6a4e5f2f9419991629be39e964d38e818ae7365c172e70b177b50932e4d4c23efe5bb5b412745fe2e48d557" },
                { "hu", "d9fd23a1400021c4b1baf51ac2b05e97f82fb28e92b7e27c4ccded2c78f754936cd79d3d393710d41223ba7602648d884b3bd01ba0eeaa17862628e6719ed3fe" },
                { "hy-AM", "b424d1e9c854e5dec567ca0c5e016e5f98c9755e7fb646dde8b6619ab12c22db8d0b0454feab8ece25d12de06657fe31aa85e45bc233eb88de267fb73081159f" },
                { "ia", "1ac318a8e3bbd9fb929600de55445d0924f40422c063ccf2dfaf2f0a33cfaf0a601672c6fe49481543f28b66c6ae9cdfcb070dd8f1f0075b2f9fa5263737088b" },
                { "id", "e00b61d6f2ffe9ce021f269700fe6cd369a718413d95038b3288e262449a8320cdff2d9745eb0b938beeb2726c9a8c00ddd57145effff5268b4bf87ee6fbc3ff" },
                { "is", "92fd70fd1e193e7e0d802879ea3380057eb3e3f636344b4f0989039e1923f5d2970260a51eadeea94c3a54c2829b5f1f042eb0bbbcc5a53088b4ee092934b711" },
                { "it", "e375f3050efd7bcceb16e319cc58817b1852c2ae6896bd80717cbeb35a7f67bf642493af656817b756b39a429e9b415e956a71b11257f519b85d2023dac6a6b5" },
                { "ja", "f7a8223ffc30c7bb6b391ce916735378e9d935a3f85118baa2f4b9fb98cbd718f1d97995704955941d74d810a83ba412a4787f4c1d229ad56893d5efff7d6a6e" },
                { "ka", "affdea3d1153d5fd89356308ec2703c609804a14b52b9014d5cf28bbbff40746deddbf9f8a648da04fa0c11825aa2a7f5bacd5586b6e01c3b05d42f57308d80e" },
                { "kab", "b7a5a059f365154e8b096ec554b914537e70cdae2032cf12aa623e3f5792416d190fc249d1ff8b43f4b2ccec9c6079bc1744217c2a1f1b20eb852ae57cb06266" },
                { "kk", "c95cd8257f439206b071a69cb6f56ef5e695912f9240ba535a1695602a88dfda686ec97bf7289024b36119e23de94f496576e99fab3f0a3025b432fdaebad310" },
                { "km", "6aae77620c425f0b06f0618faf5a88c1118ef74d02e06506d41f49d515b6ae815766dbd258756c808c02bc7aa766b23d0485c49e5c7bbacb242b729d528c92b4" },
                { "kn", "bf5f35df963d11f20f893b42aeab695c563b027751d58ffe935bfbb232519b5006ca6e3134eac19076e99ad53c10201ab7eb2e12973e88b56b9637023435a9cf" },
                { "ko", "14ec3dc20377c241b25c211d395fcbb3d47512b94631b30003a7f22f12af0bb21d33856503004f26d90d1eb08ee7890a18939f61e22275e96da2774e62e7d010" },
                { "lij", "35ced0248ff4c32daf1936c7d8b94128d1462047d3b0455a5942ab564c90eb3736f643fcfd5872b1514e1f4a8b08d6c312b0764cc4ac1cf6de7d94b57a813468" },
                { "lt", "35792b4068573d75fb89198c19bab5bb2040b68eb4d461f2a7686dc5c37dbd6008cf3660b982e49d8c6b7a5e97d06092fc9230560427ac333ffd59772d0e34e2" },
                { "lv", "e330cf76e103cec69ce64de8d3079a7c78fb0714583909eeb445f1fb54b14c8cf1b768c45b4cbb1b28395909b183ef2494c27d1398d12d11181cf8bc333e8060" },
                { "mk", "bcd36363b85db45140b7d265a9e5773d1ecaed22f71883095625207465fb11ba92fe64026adf970c0e6947cf3f3769e025600f028abc01194ae9e5262f6a9d64" },
                { "mr", "f2ec0e1be445f81b69b286cc2e99ef8f3de95d9bda1f188265d642de6e06acc7a6ff05236ea6dc82f18c5d8b7c259ce34888400cabc028da8eb3c7055222ae18" },
                { "ms", "4a246d0b163b0e58820a940c428a42c900454993f52963820e159ed84d2324a96baefb69877e5a4a8879b60b66833134bb414e1b2782f95fe750af711637a90c" },
                { "my", "314a6d4f0a49c79a4d3939164dd1766efe0877a98ffc4efa4f9a337199cec63186aefd5cdb0f9a10255590414b89027b8917257751edc2caebba5b4cd2c10d13" },
                { "nb-NO", "d7823b9243739f47580e41a874be6ea07f3e9767cd9b84539d1793a677ec7933a792591666fd0b295fb3c2e7878192097d8d99ff6b7891d1cffce91138456dc3" },
                { "ne-NP", "3e89786cb0ef37e6c9d22675fd8ebbe45d94987253c23ebf9b166cc38a390f11bcdeebaabd1d0ce6640e359a8b2174dadcc1956c1da4e6951ef97f681defc418" },
                { "nl", "6f67de98aef78f1e76c574d97cfe312fa55ca8af0331880ec98f38b483df201031308ec81e63d39da3a048a299ded0be1df3aaba1136449e7c77c58bb907de6a" },
                { "nn-NO", "bd90302d1c08a2197e5b8c3bd681464bcbeb0c37e8c4aedab50e841c6c3b9a8c5e20a500e3c46e373ca5922c5604b77e1d22cc4dad0ff0b2afcf38e62a7244e4" },
                { "oc", "2a18b339ac3eaee04c07ea3a6957e2f28346084f8656bfa1f37c9656d03cd5858a05d728762d565cf9891930517d3d3a4f647d0c290a12c9b3b920ced77d6caf" },
                { "pa-IN", "233ae320a01ffc5c70a5aecbeb674316336b1281b6c2ab57c672b6cff43fcf9f5a00ad1e9338c6cacd960fd3013f5d417c875e92502dab6108cb3bfcd24cfa86" },
                { "pl", "613123415aa6237a21ff82beafe4272a86faed119fe80fa2388bf6364fb524ccd8f9461378a5f3321db32858c91b8591b350fa0b5c69f34a009ab4dcb321cd90" },
                { "pt-BR", "38da6f80329fe195e4b55bad3459a1a2e722174c452fe40b22aea598319b3385f837dd1ed86f13f2ac9d10b1bc650e1ff0e9b951e1eaacec9e239a71842dfa6b" },
                { "pt-PT", "2d350c0b0c1146f8600ae3b44f4c690f45646d542145fb867f1c917598b11afa0e5b8fbfa7e0784eb508b089024117393c45b0ea772b4588e089608ea783e9da" },
                { "rm", "fecf12088b21f8225a9b34fd96f98348f2825e95e2659e50633689288ffd91bd53222c75c314b143d0867290e3bf1abf38a0e736623a0567d68b64a696190036" },
                { "ro", "325d8521bd6e8322bddc41b6ca670161f00cd694488238aeece8abab0eb79bb94ec4d225f197d38b2eaafb4ad6c498e2c3fa718acdcf9cd6c2605aaf79a73d19" },
                { "ru", "a8c1d346fa971e194bd35c26dcd86377c2499f258ede7f0bd5c670a8df99c41b72b2b884243b3654a93e14955605cc3cd27b168ee3f02edaa1db5c8b44a77800" },
                { "sat", "c645cbcfd4b75742f36407c83b0dedca7e8ae6b3e5881ed821cd777afa05b392c66f6112a917f4deb8fcc890e5f9eb84fae94548b5d3dbb9519990a041d96536" },
                { "sc", "011d0f7812b127a68308cd0459dbdc0474f47feebe17cbba93c1e87738da8488e547d7f0f6d1700800a6231dcef55d28a6d4abadcd57225787edd7c91e661f9b" },
                { "sco", "5ec87f2beab7e41abf37210a7e1219dff7b698d6097c15353bd21d408ed133c0d89e4d871f865a000d8dc6ade7cc61e080d12acaad0714e55fe515047b4b8035" },
                { "si", "6044a1af86f3627f25e4679fa0d3e53a5b87f3315a6f35ef33f271995282a120290fcaf3626d85e762961f46568097e17086347bb6066b71f6f17f3d4e6cda0b" },
                { "sk", "7ea04d4f39b60b91125c3407fb06f4e452fb7d504a856414584c8c9d744b8fea9f4498328633dc4bf3f367ebaad38ae63013f29338ac4b9b5bb63dc014068c48" },
                { "skr", "8a3dc85d065191fc68463f1bcc2953f56bf513063885fe69c4b440d70ceb01fcbe0aef0218232032172a31f3eb9810c87a0ead602a23c67a34c56472a960fe04" },
                { "sl", "a9dd4d94705d77a8330dbb8cd17e018d66b20c245e5cbd2f0a558eb8fc4bb1c3fee08f1057aebeaecd964183dd391f46b6cc2e064f601253cec37e2fd366bfdc" },
                { "son", "c93581580399f8d5666cc92ed225a1ad8f79c774a77ecaa89bca0286f0fecdcb496844fbe241b4ce00456253e5875a151b777c5c4f11d0713707e397b978c8a8" },
                { "sq", "da12dd9685e2928393d90637760c346662db455120b82ed5f1de9babb42379f2763240d76d39e2e0238b339fde4836ed8d3db1533fcc7cf09df44573bf12948b" },
                { "sr", "cb0f6743e92840b9ce28df9d38911ecb0fe06e2e9d7a4400bd3adfe3ec9875d81940912d1328f89ba92f04b585734a5ff96667a8f920d6f4e1a19b4ca33dcfd2" },
                { "sv-SE", "6740a8c1dc9a5719dd7bf933b71174c9d6ea62e93ac707b4944c507e363278a5b3fe8aee33c91ec5730e4a59a542614ae402abd34b7afaea4a83ccb329e61c88" },
                { "szl", "bc7ef4587bb4bb76f3a4a8e5e8470594076e7843852b6cf309c70b1bdcd2f199dd6f00c931c3c04dc04f91222605d7a196f08ba17064d7b20bf59f251cd123bc" },
                { "ta", "34464870172d9e77730dbf9014667a097469e90e663cde9f33cadd308bcd48f14602760addc445e32a52af63a14966c4c4cfd708bd27d2ba226ed2368935ee7d" },
                { "te", "408f3d9d18fe7ec7f7a376d7889f1778f2af01d7087ae4c96f112dcb0c2ea9e4cf88f6b0a68a01866c2fe247beb4db2ff35ddd726fb78cff9bdc49988443ba05" },
                { "tg", "a3d3b274816fefdc4a855e6f55c0e77c908e7db7a6c46d7af620f57757d1a982f57e2d1e66240e24157a8a91f6e9f7531c241c628750896ad04a2a2031d197c0" },
                { "th", "c1cbea4171295ef9fded2684825be5472aadc76dc3c218869f29b98a085215efb5aa32ca9634de6b58cc7ec6138a04f9298ed183f5448850845e7357ff411331" },
                { "tl", "a36595fc7ef6c10186b4e8c545bd9279aea548662d04d86afb88ca79803a88de1eba46edc5ece3c49b7185e9c6c7f87c9b03528804ef54a86ad35938693d3f12" },
                { "tr", "3e4356b166c1fdaeafd60431acda1ef705f97b34238ddb12de185233ab81f023b29e91ba6e5524d50919537a8b1d019f7735554c374a31500ae87051a473ebd0" },
                { "trs", "8185bbf68c1d543d01cb5ff0e28157e2f0908ad9a6431924539f1f199ec949039c48d077fd086ff21bb7437cc7a7a015ac2d16afa4e4860bd0a581b2050b27ca" },
                { "uk", "ca31f68de8a9ff535c4ec356b412ad800532a580219017f643ae9dbfc5e5ecf00a5477e7e6fc89070550856520016750d0a3b78378eabfdb5940c963f9411284" },
                { "ur", "f9f1f76b0c9cd2cb51662ab2b19692d15fd25953faef6af8fddd10155eda20680150173c9287c5b44d6d50007db9cf804a8e5e9c8484182776296ec64b2c938a" },
                { "uz", "2dde3c413152e74aae0dd3219023debd901aa5208ff6eb7c5b3262fa70f6b9afddb385b568a13b950820e8f50ed9da68f5fc4d7af6375fc644cfe45cb16bc692" },
                { "vi", "6e2a404512118b39d6715504219abc54873bab061eada737a495afc0f5021d164877bdf46e7d8b90a9cc638fdc81e1399c42c4ccd60f7f4bca7e2b1de33694e3" },
                { "xh", "61ff50a19dcb79a04b043d14f7fe741f02e864925e9a5cc3d525f618bf284d99ed2a322f801d0fa33c71c46e65d1dbabe4da2de53f4a5138fc6f801f7ae53d03" },
                { "zh-CN", "c7c57ff71839dc648aed42b27dc9f5f37ee6b0156f230b9137bf752ceef6c03302e5c3f6e6b939b9cb7db7452551007da97361e395078bae4815e68e2ff75501" },
                { "zh-TW", "7a345a35764e9b2c47daf32a1eb2a41aeedb8573a274924cbff4a5abd62d9916803a4887983fe74a7eb47a7f9cad29ce7af780b35c00aab69bc3b192a4fcf738" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/129.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "bef2b03eca2a7d199d6718566b03b08b4096813a7dc8d4a1a9507681e3e66930c729c581bd94dc536a700dfa1ce7303e8ab3b438f0206d35e7c1497ea55d5e52" },
                { "af", "24b2a67c9f67499f85f87784dbbfe0aade727dca0b5ec91c9cd1581c979ae93f2941910f5d70e98af75f39cfad24e96f2d1e6242293e164371cc99b43264bd05" },
                { "an", "8f93f19b49c9746bf526b7bf769de1a5abe2116da58eb7a9c2a1f2319b5e6b1d3e6f37c4203f0477d64042f3659c2a4c1ad49c1b981285ec047f7571b0123ef7" },
                { "ar", "067a7b2e87bfbfd43d7dd84e7706ed8b661838a3246089d87d16bf11279da7950930acdf4686275147934f260073bababe2dffa48d90a10da92fe7ba735f47e3" },
                { "ast", "e2ddec64c6b1ecc9d3d3c9f7042eea7758ee4177c894e4dc73dd9e0caaf1b15b96e056d14a1999e93bf0ac5901c13c80e4fdee500fe71f95ede4120933fa8f3e" },
                { "az", "922df0e8059ea1f6a40f747ee8640b3f7dfc0a3c334fd5ec5f6f7d84b250c4768b3c02b57459521b624da2a883544e42298f2ee49ce766f875687156879b0a55" },
                { "be", "5a07ecdadc71406dffa44b23cf1619c280269d2d92c06595b707a77fafb37d90a727d401c703bd029703d3940c2ef02c3874498ae1053c94385cdf06cfe0f759" },
                { "bg", "207b126a306a82aebd6613164b61f519f99fe2c2ce8ee2f533b1db22272486373d369fe8c4b327bc684f4dd69dca682095f84ba0d36d9956439b724ed4417388" },
                { "bn", "2b951c9e3070c38121436a97e5b1e5dba63393e95271cb5a25b2fc746e4ae31cf807294959cafef559c8a8da8d96ec50f8d692a31a3d209c45fb26cf0d6379a1" },
                { "br", "69424c34e7cd4639782e8a23b62f9237a6eaab740fbeaaed34cfeb9b1896e3690e53d712ad5ae425fdc1c6293d9db2c613c9b237a30d1a6e5866c913a3b83314" },
                { "bs", "b747b10071fd22555f050e7ff3c264b977db8aad4457a662ce6da0a40666b5acb565283e804ca996c3bd1e85c2b252252801f68dc0fbb9b461ae8a524fb92de5" },
                { "ca", "d8cb3d1df3840a73674ca00d136d157a9c951af165562d39fd9bc7328a5e05e1d451987aa47232316fe2d81ac93049dd9797f037d9f3ea36d1471c32636f8c12" },
                { "cak", "e0cfecb4da25c89547a8e9d26ea5c717eedad56c734ea2cc18f05594f697639a5c7f8040f6579076daf5cc219fcb4fb62466a0f62fdbf1a6de56547010c09827" },
                { "cs", "1a8725c7ff63c3053c08bc18f4da2906cfbb77ec1e37659d0677b48f46e1de755817806c15898b8b9f8ca272180da2a5a224294576d64a4f606e6273603fae55" },
                { "cy", "bb022ca1f3a3749ab48d0556d8ef3335c26a57b5a676705c6380cd80740911bff9e0d41d1703468a795d92db82039f948ad884ab90a34db734f1426581c64f0f" },
                { "da", "470d2b37c170d6c9a16e283e4bb25deea6243e5e6a3602434e4795a0c1b0fa38dda393c3eb25d4289d2cb6bf3018396e1201ec9a696f45c159518be486488113" },
                { "de", "379183b6bd56842b13b434c8dcad116da26a111a0190e5a2e4840de656270a75f4106f0aaf86a826a7cef56aa7384b864ae4c1b6805cdf4f0d99c1f6b2626fdf" },
                { "dsb", "057f56d59bf889187dc828528fc0742c719fa1d81c1c01f000aaeeecb7e9a35e03c71f60ad44056e518549aaa7bcae10efd8c9f36c8f2fa73def53ee3b686c7f" },
                { "el", "5fd710be435c087b1fda50e5867d1113de24a2f600c8b92337d13ec0522e173c1d3d9e0b0cdcd2386b5af14ad75208f3753f9cc72e4ebd31e89f6266350af164" },
                { "en-CA", "a30643a4a12721afde6bb02198df5fd07945a3ebea35ca23b886be655c736cea90dbbc0cb0fcb8fb0aca64a3378fb6b567c91144bd399ff822a16a62b4a9ec17" },
                { "en-GB", "e31bb87fda07bae3143be97aab8ff075c3cd1f3875480743ad52bb3723d2296644c2743cccadf557125d025e710be2130bad948224154239f239c87af48d2c0d" },
                { "en-US", "378cf2050e5a85776cea82ff3f13c6560fe81e8fa174fab12806f0fd3db4a3016e908871bb67aa73a3d33187ef662dd1c11f53ff83426d13b715bc7d772077f3" },
                { "eo", "c9ce2a71689cbb6d0f6c68dadc9f95285296bca5d02d114f89c17b22726f07b3d271386a99c5c065a398be42afa99e724671ac68a91313cbeb8b8f562efa5177" },
                { "es-AR", "b95986388f5b60a764b9b64f460261aaed39af3b9636f737b38bf53beeffece674492932c24b8ced163793b931502a5c2d5789f7aba7f856508363ca626c23bb" },
                { "es-CL", "2f7141ddb3d8befcec91d72a07c89ef6afc625228bb57bddff7352e482ccfe4110287af24e3e870e1d93939577c4c28ab8495a15d2c217a950b9ddcffa12ee98" },
                { "es-ES", "1a56dd813988c313a695fb869a7ae17e8c60a6876b373f54b88fa02f86046db2f34246398d89d2f9e46eb09bb86e5725c1ca928f6f0c30d54bfec46b164f1c07" },
                { "es-MX", "cf3f0e588de0be92fd77f21e7920b12eb8cac78861f238f34fce82ec0b6671a4613fe71904f2cfe50d4164cb62c00012da522193fe87196a4a457ac5155e7ba0" },
                { "et", "aa76fb53da437e915348b381deb23b68ff3683ca766f5d2394ed2f21e91ffa2728b9d6e9e8682717c0ca7f84a00fe740e14bbba8ef3de60af0c7aff1c612dafb" },
                { "eu", "1f7da42b0d6f0709cb1ec5e3813e773b727cd7f0f3df3a289c2a8fc7176ce5110f7139960b750ca4ef0a29d260f8467f6ca32877b9752e6d4ee03c87c8e29f81" },
                { "fa", "96e023e4559287655cfb6b11d5b70ab36c4de049fe1fe27fed32738e1863fe0e57b8ec772d7521a657ce16499996867718f043aedadddd40b942597b41297947" },
                { "ff", "d458eb2396679f0ecfece9b77afc9ac84b8865e8e64ca721ff57f30b9e31a013abef7c7566e7ed57f26b302c66f59a973198d5903b4ed1a203902569bff51644" },
                { "fi", "54d4a720b233cec50da5cc3a2fc23717d8a39c3732cce0ebb792cf7c103eb3e104e3560514745ac005ed77eb22d68d9bd84597d5bbb0539462ef78faa224939b" },
                { "fr", "f2195305f01cd5df9e4ac8f8db4fc99198baafd3ccaebb9d52c7914f114fc055bdf8afa0ee3acdfeefe4d8929a28b701717bf1846e174261808e5963b6d321e8" },
                { "fur", "8ecfdef03e3491eba022c1cafeab321cff1150caca0233801307ee42e32a8377ebf1cf616a8d0f6c33118c2cbdedb6f44f94b696aac29fe24e718eebbe7d7eb3" },
                { "fy-NL", "5bed19286c9cca9dffa0032c4cb126d9d723fe6e418e0d8b76515109578ab68deddf8daefee25b34d8cd889d0c55e5971fa10b22da26939e5695963bac2fa90e" },
                { "ga-IE", "8388e20e8bac5881fec2ee1c6a43ed0c5ba4d4f1c63645d7ece35a2aacba9dbf8041cfd27731fc2e5307c77069293b9386f714179d23942a10d2bde8bde2fa44" },
                { "gd", "c2343bf157821a4d59b9c5fc82033481da4eb75e3cdf7c754e59fd4c84306f987510ca4a7a509cab0337cfdf26b928ec7e28a559e43a5a27a1dcf945d31ef7e3" },
                { "gl", "80b74c6edcffee5aad4f5a8372ffd8f14ef0f1b621ff9c90eb855e61861d7a2f6e3b510cf60868adfadc82d94e8c2024e49f04dc7fb889d5088e11a054707988" },
                { "gn", "6e3cea89f9c20a167bbdb856887259f8890c2eb3a0a48cb61d4682da82974af566a054f1c5ac588a5d5ba8cf8ff0bce44ebafa011deda0139d9e8efa0fe064c4" },
                { "gu-IN", "e58ef4269cd4d65b40af59434be9e2d466eca6866c0a3494fb14fba353941cd6edcb2fc2511f576dd5efa5e94da75e116542327475113f9a376ea45c0d68378f" },
                { "he", "c2adce9770d349e1cc68c6ea34b95be46a47029eeb80db967607c288f3a8a225e794b11300c7feac47450b1f5d8666805634c87efef743a386e44a7fb2d08e19" },
                { "hi-IN", "b621f7620d15b853c6e040e9209a7cbf82403627a1325dd46316e803f2abc320d366ea2e85e4700c27d6cb336f54b97ecc787e0894c6b4a35ab60a76ff69ad34" },
                { "hr", "da06aacbe1be1625574ffd7d1d42789be08971aaab862fd2aee50851dde6cbc13bbb821fe4b83d6493f10a037830640f2e2b2a51725a6f92e5b6498c306ed782" },
                { "hsb", "5991922513273949c3ac6efce43ca26b31d7e8b21944d242c489098f1f6a2195e489169ddd9fbf7035e5fece3c69679be769df3e550d461a21c35688156d5b4e" },
                { "hu", "b68fa2d09fc9f034605043ef5b4c25f3268cbe0e9299fb0708d1d7995c3daf3071e9623043bcaaef237165087afb8122e42189ff36efd470b82893cda4459d5c" },
                { "hy-AM", "a71c910b07ea50f5fe1a6021690ff3a0c2d978245bf2c2c752d986e600f3ce99f5fa64110cf62b52a4b8ebc3befbc16c24621a561e71d4e32f1c4d0ce64b638f" },
                { "ia", "a80c839af386ff8096152edc4424902ee3f6608204916fcb5276c860cda033d1c0d755b61abb5811284c976e558ece5c390a05d6bba555999c024bfba6117916" },
                { "id", "59d5fa604baa4651fd6719a3e1c053901a28468a57c2b9f9f6fe67759465a2efaa5339e8fe62f136b6529b961c57117839d937c9f6eb516e060550d6cd927a19" },
                { "is", "7883cd89b17529bc57d80be14856807172e3f9e65f5aaea0cc7e28561044659af7bede8741f7e53c8523165870ed7c0651f76a13f191a81688a910d2afb9d930" },
                { "it", "d4cf2648a20d86d18d69c4a2f533f4899ddfeabef4dd4deb2793c5f05f0f32d28c400f5f96f30dfad91cb4eccfb271d0e4639208c28929b0aa5a3827b1cfe9b4" },
                { "ja", "16ab572c2560157e14c5fd936bb0bed8b92701434780925fbc32126bbee939cdeab4926191053fa9fa4de9d1da4e293c59b97e895a0e1fd06e000df03cadcf3a" },
                { "ka", "0f94fed222df95869e639959432d1f9ad89cdf4a1fee78e6e300dbac11d019881e34142648286c3311c4a1302f467a53451b1707774677a2a294871a0166664c" },
                { "kab", "eeb26e4fd2ac33f5989b4f762c29a7b8cbd3d9682e65f70f2f8e04d2d0216e7ad1f51a5976f964c3c6864e795402a96b0a4e64010135d89ebfa2ab6522461740" },
                { "kk", "6c07fce09f0daa7559c6135a30fa1373801a2f0280600cf9b43538eb6b847050671a53153f548182b53636f742c56e36c733f6cf0092983cb145f36da65770d7" },
                { "km", "cb0bf421ab5aa041fde8c53ac394675a3514bba8a94317db9db7cb2c6e698b8edb8545d385312395d0ac78764cce86a6ddce4fc49b439eeb273e0fa08712d5ce" },
                { "kn", "ae15a65d95b25a48cfe5e3af83ede7dcdd98167953a9f767d7fa47df5934086213b21883c92a2370c54a21d38cdc5db866c304e00a4f7d2ae061140f2143f56c" },
                { "ko", "ea7e5bcb7f307dc698768e687bce54c5d52c562cba0b747395827f42e07388f08bfb2f66254337aef17ba2180f5722a5eaa464b3a7933a37e32cbc66b1820a89" },
                { "lij", "a6a2b204874cb3d979a1e92615b1239f13906ca9ff035f6cad6cc218ea361b7e205cce8cedcb11cd6cf39810421d5eb854eae979120a9eb67afb555fba657e2c" },
                { "lt", "39857ae4ab9437b34c6f3a61ed22399467030a96e8be538d701f69e0d9f46cd0376a2c6b5e1bdfc32147d6c33e32e45baeb7cd6852fb6d21db39be98f1c0b38a" },
                { "lv", "42f9ea18d335a6fa96801b7130804d6950e5a1fc2f7125d620c9cad5f1ae79c7278aa7872706b0936d4b15a5a2444e870439935031dc309b5b09052ce09ba127" },
                { "mk", "b7e687671e0e72e51b8a7d003aafc705bb2170db4da6db0443f97a96dc05661a399d0160ea78540ceb4e9fbd23387d4b60baab39ff5c2a8a6a7dd3c1eb3da21d" },
                { "mr", "c2f4d41cae45993e1eb1e6d877a5dcab8096f0daf1f2d0dffdcb60b3cb8bff181f97ebc5eed9af199ce84aea0e1410360aaf27189b0c76d90141656cd37a170d" },
                { "ms", "09f6d2dbc9b1b3a81b12c1cce9af099db40c32ef3834b61119df911e08ece2a850e5ab948c8d772f855824431a187e662ebeeec768f38a2e3457394b814761f1" },
                { "my", "027ad08a1d2b91820f30a1610444714a53607029bcf70ba2b238921ee054c669a75b4beccdae4af605fcfdcc118b5b90fe2586596cb4687f12d78fbab3bcc03d" },
                { "nb-NO", "e47dc5807c2cf636ade0a1ed348cef9109e4480d8a9a7b6c4068770761aeccced259d9e20a366918c041ed06ee1b6f34a286b8be1b8813b5e8474f5d39edb178" },
                { "ne-NP", "1bca5b449ea9ed04e486a532b502985e6ac34528c54a39c7ab81860c43cc8c8b1ecb64cb5433fafc2c10fb3d10cb663054d9b46b1440da33bca8129c63f62385" },
                { "nl", "14df7e476e058f1168d3efa869092f9f9d71ea76040d726a3f2538f6fda5b0428aadfbd807bb00d3d09e5887148b1b35fe989b884bcaa506c09bc7710168bc57" },
                { "nn-NO", "9da9d473c969799ad48a992400409875c167d380da66bcd6aa761772ae0290e5d6c586be78a80a9165bd8d80372968b253994f8fef62a0bea8a6e23271e9766d" },
                { "oc", "1273d0606b67b9322fff122847f66cd6ff48fb7c81083906e63f6b7fd81bc73c9d508b2f1936ad6e0fd5de9a39e861ddd6cc6991b270cb18dab20c2ffef49fbf" },
                { "pa-IN", "42d4060957228b398bf23cfdbc1db355817aa295ee78ab881253f6c69f46ddabf191d5edc8edb7c54de223ef37a69bf6c5555a3c2fb7691d5f995bc24c868715" },
                { "pl", "ab30eda7442141348f82f71ceb89928f8c24ef6384c09eef582c00b80f2305e2c953c4fe2f7ae6caab8c2e5c775ed0605216d4fb1274b90919e781fec47e279c" },
                { "pt-BR", "dfab85e3a603c13e1610cac2d65918a87c7250aa118f13eb36cdfcb7286407a3e87ba939204c811d4dd138ef90670918bd3ead2f4da5c9e6d077f11444e4f70c" },
                { "pt-PT", "0ecea2844ed2a8069ecf19cb267afc2d87b18bbc6cf069f5f94f0d848fb0267084c043e8a3c387d57689ce4b58347d761fb91dc1601dbf80c21808859aa0b753" },
                { "rm", "adf0f908141c633fd9ac65ceac5f8ecb95e9dd67a07b24422147653ffc573863753b6ca2944824c621516969ea812a82d8c1b5bc07e468e1e6e3033b474d047f" },
                { "ro", "b6a56c9d6325dc7434ade3600068cab522a4a622f658a5feb5aaff8d98e26b6c7f3d4059ba47f2bbbb708a14a72f5f5846c4ac0a1beb35c13acfe99e14fb192c" },
                { "ru", "3c390dead8d205b11fe2559788f4fce40fe9efadd0d380388f93d00fafefc68cd31210ead66effd82b2e67d212874dc6e91a4018210daaebce39e0e7c9be5ac0" },
                { "sat", "943dc52976f6035696620f33edd61b75c01d97c74f5e5e85fb57232301b0cc53dcd6145115b473764b0ad3dbd5464500aca5cc1faa93a69e0cae4199501d97a7" },
                { "sc", "64ff34e0e4c9a1aa6369ffca26464c89d55809ff163200dbe0bcd34b640ad0edf8e24a7876e2b86f545c8aacccfe1afb64553463c3be38f7ca22e60dc8bbf5f1" },
                { "sco", "74b2e389b4575d942ef7705e1e778e8194b0661b36f1ab32e1ca8c30c8a64657d55678fe574aea002ba06c345736e4f2f98b5fe3ceb845b75ac617700f5ec6df" },
                { "si", "ad3729c33d4074cba96445bbecba70865a9d2d3a252ea3cf170a5c95a7104866a2e561c8e5e83d98afee613b4702bb2eabd5ea64226c8c2640992f354bb4f08d" },
                { "sk", "9f395a0100f379c3a26524654100673aed472996c512daaccd765f13618ef577ac67f802cc7128c588444087b5241985ffe15ca50d92f9937ba201629f1e6ed3" },
                { "skr", "d5d30ce117a89104821dc6470af41a00f1a234bb92b5c40ef0d8af670ffc873d03e67cdddea4f0086d367defd98c950278a3e5dec3f4b2974c09a3e91e3cfe5f" },
                { "sl", "568de80f8b19f6b89aa89dc42fd3407983d04b3108b580b11214ab47c841e2fdf6b054576abf239b0780e9f5b9eb10c73f230e3c007d17f510a5a869a2d980c3" },
                { "son", "d0fb099d910d070bc09f8e63d253322a72f2abfa694a2b575d545ba95a0581efc7820a65553777bcc509eb0595062a443207f79f17b675dea1c59628151f0ff9" },
                { "sq", "ed47a3b3c23ed5f0d2cea0271dbc420a8256687a443baec4b8b686413647162bbe52639b455f5726284dd18eddf329788657a8f2c7458032546f9bb1e3cf825d" },
                { "sr", "ec9ec227e93215d3805858b4765ff964c3e94e8c96d9268278e3b9f5d9fa632be4f98fd9b24946ca6fe79d5902fe4b8c471e5b6330c53605bdc5392ad156277e" },
                { "sv-SE", "7e632a48aecd4aa371183e1956d08a4038e2cf4ddccbeb7eaca406ee954dcb54b1b8b7017be4b965fc187523ff915f9318e91c4a5c3a48408c42ee20cad6e296" },
                { "szl", "61c36782de04d2554f45e772d41d59487e88e126b4a0e50d29ae799f557ff6fca9fc0f5bdc81cbacd1bb9d458db139151798a658857a7f424ec675c513ef26f1" },
                { "ta", "7cc682d2bdf450f541ea2b1eb9352a97bf89def3768c8c4cd3c1ce90d25a2e3a6b51013cf9e6ba74a249b2b0bc579307fe380b4b055aa2b057cc7afb2aad8717" },
                { "te", "c3b858c82af2957f9a5daabe1bd56c3a1a988415d6d30d43f1de1c18d97916a3d097cae039b8272334a0c11d02225816b6792e277525ef1d983ae0eb580a55f8" },
                { "tg", "ed986e5f8e77272a03c47354b8145fe7c428265baf65131cfef3446d2b89effbf7986da5ad3991bcd98996d822dbee05264ed2a7a4be78f1f1aa524c1102898b" },
                { "th", "c647f3145b2744b434e724d42a80cc094053e194a25da2c4b7c3b12769f78141ca537e04d53c195c91e40d0f915edbcb61b59448e9009c5e0c401bf5f83427d7" },
                { "tl", "0475ed9bfaa8eab3e07166a9ee7f0f7433009add31fd8d5a21f833b7caf2fcb499ad7d1e2727f68788a7463d78244e3827a180a12bbfeb938243c75c8469096d" },
                { "tr", "782ab48416fdd6e41ccb5d20a1063aa58aa1f3661a366efa2777d33f399958ed564f17fcc0e33033a905f62822c5d487cb9135ae90b79ee7a75845406cdadedf" },
                { "trs", "df8d62e7148b80ca7dc76717f09ba599e4b48688b85e736ed0f8eb5eaab8f08806de3800dbb18072536d1c69612e3637e1fa44e11099f728af5ca00dec86d09c" },
                { "uk", "9435b82aedfff509631df639f9a1ed812f1ef752dd310bfe92ea3c116c48cf3647c12443115404c8b0ae2b5e118eea41e84f7b65a51642233602166cbf291299" },
                { "ur", "740986b764e7127c6df3bb315fe35cbc12811148bee9be273c0b7a6a3d21caaaacfc9e6d334cf1753821a92c52cbc63c31d94a933c2baecdd03daade081e8365" },
                { "uz", "943abc7da2f280e0b9bfd90a40ea14035449fbdbae2fb7baac6a1ed342981fa3fc399433e86b9a40261c50b72c3ae41e441512a65dfb9897549604d9ae830be0" },
                { "vi", "727322c4dbd9af87b24c177e97274e39821092fb2bf98078801e7f7871ca3b8c3934412204fc3866297d3d1a67ad5cbbe0ecf898767ca7f7ef99fafa64af5374" },
                { "xh", "982b53e03037bba70281c840cc24074581a5a4d36fb4b3199531a0577ddf821df56152b0609422d5748d68e386fd1ad258d96b94c07755060f2520d27bf3112b" },
                { "zh-CN", "bff645093b2e07e4351cbf71880d2e37fb01036d0864dd3a66fa50bf2b4c36f62e3975cc9fb60924ccd20e9a87e1f2bbe05313f82f05d7581ac2f4bf8eb42c7c" },
                { "zh-TW", "72dd2fdab396c92601a1567e1047b28cf3f73a5e05ed558a93b0f236a0ee81a0050cad73110c56c06b1ee116ca529c0605d723a26a47226028efafb5117c7c0a" }
            };
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
            const string knownVersion = "129.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
